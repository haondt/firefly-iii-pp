using Firefly_iii_pp_Runner.Converters;
using Firefly_iii_pp_Runner.Exceptions;
using Firefly_iii_pp_Runner.Models.ThunderClient;
using Firefly_iii_pp_Runner.Models.ThunderClient.Dtos;
using Firefly_iii_pp_Runner.Models.ThunderClient.Enums;
using Firefly_iii_pp_Runner.Settings;
using Firefly_pp_Runner.Extensions;
using FireflyIIIpp.Core.Exceptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace Firefly_iii_pp_Runner.Services
{
    public class ThunderClientEditorService
    {
        private readonly ThunderClientEditorSettings _settings;
        private JsonSerializerSettings _serializerSettings;

        public ThunderClientEditorService(IOptions<ThunderClientEditorSettings> options)
        {
            _settings = options.Value;
            if (!Directory.Exists(_settings.Path))
                throw new Exception($"Path does not exist: {_settings.Path}");

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.ConfigureFireflyppRunnerSettings();
        }

        public async Task<List<string>> GetFolderNames()
        {
            var collections = await LoadCollections();
            var collection = collections.FirstOrDefault(c => c.ColName == _settings.CollectionName, null);
            if (collection == null) throw new NotFoundException($"Unable to find collection with name {_settings.CollectionName}");

            var folderDict = collection.Folders.ToDictionary(f => f.Id, f => (f.Name, f.ContainerId));
            var pathDict = new Dictionary<string, (bool IsRoot, string Path)>() { { "", (true, "root")} };

            foreach(var kvp in folderDict)
            {
                var parentId = kvp.Value.ContainerId;

                if (!pathDict.ContainsKey(parentId))
                {
                    var depth = 0;
                    var maxDepth = 100;
                    var idStack = new Stack<string>();
                    while (!pathDict.ContainsKey(parentId))
                    {
                        idStack.Push(parentId);
                        parentId = folderDict[parentId].ContainerId;

                        depth++;
                        if (depth > maxDepth)
                            throw new Exception("Exceeded max depth while attempting to assemble folders");
                    }

                    while(idStack.Count > 0)
                    {
                        var id = idStack.Pop();
                        var _parentPath = pathDict[parentId];
                        if (_parentPath.IsRoot)
                            pathDict[id] = (false, $"{folderDict[id].Name}");
                        else
                            pathDict[id] = (false, $"{_parentPath.Path}/{folderDict[id].Name}");
                        parentId = id;
                    }
                }

                var parentPath = pathDict[parentId];
                if (parentPath.IsRoot)
                    pathDict[kvp.Key] = (false, $"{kvp.Value.Name}");
                else
                    pathDict[kvp.Key] = (false, $"{parentPath.Path}/{kvp.Value.Name}");
            }

            return pathDict.Where(kvp => !kvp.Value.IsRoot).Select(kvp => kvp.Value.Path).ToList();
        }

        public async Task SortTests()
        {
            var collections = await LoadCollections();
            var collection = collections.FirstOrDefault(c => c.ColName == _settings.CollectionName, null);
            if (collection == null) throw new NotFoundException($"Unable to find collection with name {_settings.CollectionName}");
            var clients = await LoadClients();

            Sort(collection, clients);

            SaveCollections(collections);
            SaveClients(clients);
        }

        private void Sort(Collection collection, List<Client> clients)
        {
            var names = collection.Folders.Select(f => f.Name)
                .Concat(clients.Select(c => c.Name))
                .Distinct().OrderBy(s => s)
                .Select((s, i) => (s, 100 + i * 10))
                .ToDictionary(t => t.s, t => t.Item2);

            foreach(var folder in collection.Folders)
                folder.SortNum = names[folder.Name];
            foreach(var client in clients)
                client.SortNum = names[client.Name];
        }

        private Folder GenerateFolderPath(Collection collection, CreateTestCaseFolderModeEnum createFolderMode, string folderName)
        {
            if (folderName.Split('/').Select(s => s.Trim()).Any(s => string.IsNullOrWhiteSpace(s)))
                throw new ArgumentException($"Invalid folder path: {folderName}");

            var path = folderName.Split('/').Select(s => s.Trim());
            Folder? parent = null;
            foreach (var name in path)
            {

                Folder? folder = null;
                if (createFolderMode == CreateTestCaseFolderModeEnum.UseExistingOrCreate)
                    folder = collection.Folders.FirstOrDefault(f => f.ContainerId == (parent?.Id ?? string.Empty)
                    && f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (folder == null)
                {
                    folder = new Folder
                    {
                        Name = name,
                        ContainerId = (parent?.Id ?? string.Empty)
                    };

                    collection.Folders.Add(folder);
                }

                parent = folder;
            }

            if (parent == null)
                throw new ArgumentException($"Unable to resolve folder name {folderName} with create folder mode {createFolderMode}");

            return parent;
        }

        public async Task<(Folder Folder, Client Client)> CreateTestCase(CreateTestCaseRequestDto request)
        {
            var collections = await LoadCollections();
            var clients = await LoadClients();

            var collection = collections.FirstOrDefault(c => c.ColName == _settings.CollectionName, null);
            if (collection == null) throw new NotFoundException($"Unable to find collection with name {_settings.CollectionName}");

            var folder = GenerateFolderPath(collection, request.CreateFolderMode, request.FolderName);

            if (request.ConfigureExpectedValues)
            {
                var existingTests = folder.Settings.Tests.ToHashSet();
                foreach(var kvp in request.ExpectedValues)
                {
                    existingTests.Add(new Test
                    {
                        Type = TestTypeEnum.JsonQuery,
                        Value = kvp.Value,
                        Action = TestActionEnum.Equal,
                        Custom = $"json.{kvp.Key}"
                    });

                }

                folder.Settings.Tests = existingTests.ToList();
            }

            var clientNames = clients.Where(c => c.ContainerId == folder.Id).Select(c => c.Name).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            var clientNum = 1;
            var clientName = $"{folder.Name} {clientNum}";
            while (clientNames.Contains(clientName))
            {
                clientNum++;
                clientName = $"{folder.Name} {clientNum}";
            }

            var client = new Client
            {
                ColId = collection.Id,
                ContainerId = folder.Id,
                Name = clientName,
                Url = "{{base_url}}/apply",
                Method = "POST",
                Body = new Body
                {
                    Type = ClientBodyTypeEnum.Json,
                    Raw = JsonConvert.SerializeObject(request.BodyFields, _serializerSettings),
                }
            };

            clients.Add(client);

            Sort(collection, clients);

            SaveCollections(collections);
            SaveClients(clients);

            return (folder, client);
        }

        public async Task ImportPostmanFile(string json)
        {
            var postmanCollection = JsonConvert.DeserializeObject<Models.Postman.Collection>(json, _serializerSettings);
            var existingCollections = await LoadCollections();
            var collection = existingCollections.FirstOrDefault(c => c.ColName == _settings.CollectionName, null);
            if (collection == null) throw new NotFoundException($"Unable to find collection with name {_settings.CollectionName}");
            var clients = (await LoadClients()).ToDictionary(x => x.Id, x => x);

            var collectionId = collection.Id;
            foreach (var item in postmanCollection.Item)
                TransferPostmanItemToThunderClient(item, collection, clients);

            SaveCollections(existingCollections);
            SaveClients(clients.Select(kvp => kvp.Value).ToList());
        }

        private void TransferPostmanItemToThunderClient(Models.Postman.Item item, Collection collection, Dictionary<string, Client> clients)
        {
            TransferPostmanItemToThunderClient(null, item, collection, clients);
        }

        private void TransferPostmanItemToThunderClient(Models.Postman.Folder? parent, Models.Postman.Item item, Collection collection, Dictionary<string, Client> clients)
        {
            if (!_settings.KeepPostmanIds)
                item.Id = Guid.NewGuid();

            if (item is Models.Postman.Request request)
            {
                var thunderItem = new Client
                {
                    Id = request.Id.ToString(),
                    ColId = collection.Id,
                    ContainerId = parent?.Id.ToString() ?? string.Empty,
                    Name = request.Name,
                    Url = request.RequestInner.Url.Raw,
                    Method = request.RequestInner.Method,
                    SortNum = 100,
                    Body = new Body
                    {
                        Type = ExtractClientBodyTypeEnum(request.RequestInner.Body),
                        Raw = request.RequestInner.Body.Raw,
                    }
                };

                var (hasTests, tests) = ExtractTests(request);
                if (hasTests)
                    thunderItem.Tests = tests;


                clients[thunderItem.Id] = thunderItem;

            } 
            else if (item is Models.Postman.Folder folder)
            {
                var thunderItem = new Folder
                {
                    Id = folder.Id.ToString(),
                    Name = folder.Name,
                    ContainerId = parent?.Id.ToString() ?? string.Empty,
                    SortNum = 100,
                };

                var (hasTests, tests) = ExtractTests(folder);
                if (hasTests)
                    thunderItem.Settings.Tests = tests;

                if (_settings.KeepPostmanIds)
                    collection.Folders.RemoveAll(x => x.Id.Equals(thunderItem.Id, StringComparison.InvariantCultureIgnoreCase));
                collection.Folders.Add(thunderItem);

                foreach (var child in folder.Item)
                    TransferPostmanItemToThunderClient(folder, child, collection, clients);
            } 
            else
            {
                throw new ArgumentException("Unable to convert postman object {object}", item.GetType().FullName);
            }

        }

        public (bool HasTests, List<Test>? Tests) ExtractTests(Models.Postman.Item item)
        {
            var tests = item.Event.FirstOrDefault(e => e.Listen == Models.Postman.Enums.EventTypeEnum.Test, null);

            if (tests == null || tests.Script == null)
                return (false, null);

            if (tests.Script.Type != Models.Postman.Enums.ScriptTypeEnum.Javascript)
                throw new ArgumentException($"Unsupported script type: {tests.Script.Type}");

            var exec = string.Join(string.Empty, tests.Script.Exec);
            var regex = new Regex(@"pm\.test\(['""](?<name>[\w\s]+)['""][^{]*{[^}]+pm\.expect\(jsonData\.(?<key>[\w_]+)\).to.eql\(\s*(?<value>.*?)\);");
            var matches = regex.Matches(exec);
            string convertJsonValue(string input) {
                var val = JsonConvert.DeserializeObject(input);
                switch (val)
                {
                    case string:
                    case DateTime:
                        return JsonConvert.DeserializeObject<string>(input);
                    default:
                        return JsonConvert.SerializeObject(val);
                }
            }

            var thunderTests = matches.Where(m => m.Success)
                .Where(m => m.Groups.ContainsKey("name")
                    && m.Groups.ContainsKey("key")
                    && m.Groups.ContainsKey("value"))
                .Select(m => new Test
                {
                    Type = TestTypeEnum.JsonQuery,
                    Custom = $"json.{m.Groups["key"].Value}",
                    Action = TestActionEnum.Equal,
                    Value = convertJsonValue(m.Groups["value"].Value)
                }).ToList();

            return (true, thunderTests);
        }

        private ClientBodyTypeEnum ExtractClientBodyTypeEnum(Models.Postman.Body postmanBody)
        {
            switch (postmanBody.Mode)
            {
                case Models.Postman.Enums.RequestBodyModeEnum.Raw:
                    switch (postmanBody.Options.Raw.Language)
                    {
                        case Models.Postman.Enums.RequestBodyOptionsLanguageEnum.Json:
                            return ClientBodyTypeEnum.Json;
                        default:
                            throw new ArgumentException($"Unable to convert Postman body language option {postmanBody.Options.Raw.Language}.");
                    }
                default:
                    throw new ArgumentException($"Unable to convert Postman body mode {postmanBody.Mode}.");
            }
        }

        public async Task<int> GetClientCount()
        {
            return (await LoadClients()).Count();
        }

        private  Task<List<Client>> LoadClients()
        {
            return ReadFromFileAsync<List<Client>>(Path.Combine(_settings.Path, _settings.ClientFile));
        }
        private void SaveClients(List<Client> clients)
        {
            WriteToFile(Path.Combine(_settings.Path, _settings.ClientFile), clients);
        }

        private Task<List<Collection>> LoadCollections()
        {
            return ReadFromFileAsync<List<Collection>>(Path.Combine(_settings.Path, _settings.CollectionFile));
        }

        private void SaveCollections(List<Collection> collections)
        {
            WriteToFile(Path.Combine(_settings.Path, _settings.CollectionFile), collections);
        }

        private async Task<T> ReadFromFileAsync<T>(string path)
        {
            if (!File.Exists(path))
                throw new Exception($"File does not exist: {path}");
            using var reader = new StreamReader(path, new FileStreamOptions
            {
                Access = FileAccess.Read,
                BufferSize = 4096, 
                Mode = FileMode.Open,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            var text = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<T>(text, _serializerSettings);
        }
        private void WriteToFile<T>(string path, T obj)
        {
            if (!File.Exists(path))
                throw new Exception($"File does not exist: {path}");
            using var writer = new StreamWriter(path, new FileStreamOptions
            {
                Access = FileAccess.Write,
                BufferSize = 4096, 
                Mode = FileMode.Create,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            var serializer = JsonSerializer.CreateDefault(_serializerSettings);
            serializer.Serialize(writer, obj);
        }
    }
}
