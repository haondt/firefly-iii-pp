using Firefly_pp_Runner.Extensions;
using FireflyIIIppRunner.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Persistence
{
    public class FilePersistenceService : IPersistenceService
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public FilePersistenceService()
        {
            _serializerSettings = new JsonSerializerSettings().ConfigureFireflyppRunnerSettings();
        }
        public bool CollectionExists(string collection)
        {
            return Directory.Exists(collection);
        }

        public void AssertCollectionExists(string collection)
        {
            if (!CollectionExists(collection))
                throw new Exception($"Directory does not exist: {collection}");
        }

        public bool PathExists(string collection, string path)
        {
            return File.Exists(Path.Combine(collection, path));
        }

        public void AssertPathExists(string collection, string path)
        {
            if (!PathExists(collection, path))
                throw new Exception($"File does not exist: {Path.Combine(collection, path)}");
        }

        public async Task<T> ReadAsync<T>(string collection, string path)
        {
            AssertPathExists(collection, path);
            using var reader = new StreamReader(Path.Combine(collection, path), new FileStreamOptions
            {
                Access = FileAccess.Read,
                BufferSize = 4096, 
                Mode = FileMode.Open,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            var text = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<T>(text, _serializerSettings);
        }

        public Task WriteAsync<T>(string collection, string path, T obj)
        {
            AssertPathExists(collection, path);
            using var writer = new StreamWriter(Path.Combine(collection, path), new FileStreamOptions
            {
                Access = FileAccess.Write,
                BufferSize = 4096, 
                Mode = FileMode.Create,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan
            });

            var serializer = JsonSerializer.CreateDefault(_serializerSettings);
            serializer.Serialize(writer, obj);
            return Task.CompletedTask;
        }

        public Task CreateCollectionAsync(string collection)
        {
            if (Directory.Exists(collection))
                throw new ArgumentException($"Collection already exists: {collection}");
            Directory.CreateDirectory(collection);
            return Task.CompletedTask;
        }

        public Task CreatePathAndWriteAsync<T>(string collection, string path, T obj)
        {
            AssertCollectionExists(collection);
            if (PathExists(collection, path))
                throw new ArgumentException($"Path already exists: {path}");
            File.Create(Path.Combine(collection, path));
            return WriteAsync(collection, path, obj);
        }
    }
}
