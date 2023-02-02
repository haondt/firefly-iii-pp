using Firefly_iii_pp_Runner.API.Converters;
using Firefly_iii_pp_Runner.API.Exceptions;
using Firefly_iii_pp_Runner.API.Models.ThunderClient;
using Firefly_iii_pp_Runner.API.Models.ThunderClient.Enums;
using Firefly_iii_pp_Runner.API.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace Firefly_iii_pp_Runner.API.Services
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

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new ItemConverter()
                },
                MissingMemberHandling = MissingMemberHandling.Error,
            };
        }

        public async Task ImportPostmanFile(string json)
        {
            var postmanCollection = JsonConvert.DeserializeObject<Models.Postman.Collection>(json, _serializerSettings);
            var existingCollections = await LoadCollections();
            var collection = existingCollections.FirstOrDefault(c => c.ColName == _settings.CollectionName);
            var clients = (await LoadClients()).ToDictionary(x => x.Id, x => x);

            if (collection == null)
                throw new NotFoundException($"Unable to find collection with name {_settings.CollectionName}");

            var collectionId = collection.Id;
            foreach (var item in postmanCollection.Item)
                TransferPostmanItemToThunderClient(item, collection, clients);

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
                    ContainerId = parent != null ? parent.Id.ToString() : "",
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

                var tests = request.Event.FirstOrDefault(e => e.Listen == Models.Postman.Enums.EventTypeEnum.Test);

                if (tests != null)
                {
                    if (tests.Script.Type != "text/javascript")
                        throw new ArgumentException($"Unsupported script type: {tests.Script.Type}");

                    thunderItem.Tests = ExtractTests(tests.Script.Exec);
                }

                if (parent != null)
                {

                }


            }

        }

        public List<Test> ExtractTests(List<string> postmanExec)
        {
            var exec = string.Join(string.Empty, postmanExec);
            var regex = new Regex(@"pm\.test\(['""](?<name>[\w\s]+)['""][^{]*{[^}]+pm\.expect\(jsonData\.(?<key>[\w_]+)\).to.eql\(\s*(?<value>.*?)\);");
            var matches = regex.Matches(exec);
            string convertJsonValue(string input) {
                var val = JsonConvert.DeserializeObject(input);
                if (val.GetType() == typeof(string))
                    return (string)val;
                return JsonConvert.SerializeObject(val);
            }
            return matches.Where(m => m.Success)
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

        private async  Task<List<Client>> LoadClients()
        {
            var filePath = Path.Combine(_settings.Path, _settings.ClientFile);
            if (!File.Exists(filePath))
            {
                throw new Exception($"File does not exist: {filePath}");
            }
            
            using (var reader = new StreamReader(filePath))
            {
                return JsonConvert.DeserializeObject<List<Client>>(await reader.ReadToEndAsync(), _serializerSettings);
            }
        }
        private async  Task<List<Collection>> LoadCollections()
        {
            var filePath = Path.Combine(_settings.Path, _settings.CollectionFile);
            if (!File.Exists(filePath))
            {
                throw new Exception($"File does not exist: {filePath}");
            }
            
            using (var reader = new StreamReader(filePath))
            {
                return JsonConvert.DeserializeObject<List<Collection>>(await reader.ReadToEndAsync(), _serializerSettings);
            }
        }
    }
}
