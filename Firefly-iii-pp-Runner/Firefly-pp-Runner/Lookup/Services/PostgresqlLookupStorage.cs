using Haondt.Core.Models;
using Haondt.Identity.StorageKey;
using Haondt.Persistence.Postgresql.Services;
using Haondt.Persistence.Services;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Firefly_pp_Runner.Lookup.Services
{
    public class PostgresqlLookupStorage(IOptions<PostgresqlStorageSettings> options) : PostgresqlStorage(options), ILookupStorage
    {
        private readonly PostgresqlStorageSettings _settings = options.Value;
        public async Task<Result<StorageResultReason>> AddForeignKey<T>(StorageKey<T> primaryKey, StorageKey<T> foreignKey)
        {
            await WithConnectionAsync(async (connection) =>
            {
                var query = $@"
                    INSERT INTO {_foreignKeyTableName} (ForeignKey, PrimaryKey)
                    VALUES (@foreignKey, @primaryKey)
                    ON CONFLICT (ForeignKey, PrimaryKey) DO NOTHING;";
                if (_settings.StoreKeyStrings)
                    query = $@"
                        INSERT INTO {_foreignKeyTableName} (ForeignKey, KeyString, PrimaryKey)
                        VALUES (@foreignKey, @foreignKeyString, @primaryKey)
                        ON CONFLICT (ForeignKey, PrimaryKey) DO NOTHING;";

                await using var foreignKeyCommand = new NpgsqlCommand(query, connection);
                foreignKeyCommand.Parameters.AddWithValue("@primaryKey", StorageKeyConvert.Serialize(primaryKey));
                foreignKeyCommand.Parameters.AddWithValue("@foreignKey", StorageKeyConvert.Serialize(foreignKey));

                if (_settings.StoreKeyStrings)
                    foreignKeyCommand.Parameters.AddWithValue("@foreignKeyString", foreignKey.ToString());

                await foreignKeyCommand.ExecuteNonQueryAsync();
            });

            return new Result<StorageResultReason>();
        }

        public async Task<Result<int, StorageResultReason>> DeleteForeignKey<T>(StorageKey<T> foreignKey)
        {
            var keyString = StorageKeyConvert.Serialize(foreignKey);
            var rowsAffected = await WithConnectionAsync(async connection =>
            {
                string query = $"DELETE FROM {_foreignKeyTableName} WHERE ForeignKey = @key";
                await using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@key", keyString);
                return await command.ExecuteNonQueryAsync();
            });

            if (rowsAffected == 0)
                return new Result<int, StorageResultReason>(StorageResultReason.NotFound);
            return new Result<int, StorageResultReason>(rowsAffected);
        }

        public async Task<List<StorageKey<T>>> AutocompleteForeignKey<T>(StorageKey<T> partialForeignKey, int maxResults)
        {
            var partialKeyString = StorageKeyConvert.Serialize(partialForeignKey);
            var prefix = $"{partialKeyString}%";

            var results = new List<StorageKey<T>>();
            return await WithConnectionAsync(async connection =>
            {
                string query = $"SELECT ForeignKey FROM {_foreignKeyTableName} WHERE ForeignKey ILIKE @prefix LIMIT {maxResults}";
                await using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@prefix", prefix);

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var resultKeyString = reader.GetString(0);
                    var resultKey = StorageKeyConvert.Deserialize<T>(resultKeyString);
                    results.Add(resultKey);
                }
                return results;
            });
        }
        public async Task<List<StorageKey<T>>> AutocompletePrimaryKey<T>(StorageKey<T> partialPrimaryKey, int maxResults)
        {
            var partialKeyString = StorageKeyConvert.Serialize(partialPrimaryKey);
            var prefix = $"{partialKeyString}%";

            var results = new List<StorageKey<T>>();
            return await WithConnectionAsync(async connection =>
            {
                string query = $"SELECT PrimaryKey FROM {_primaryTableName} WHERE PrimaryKey ILIKE @prefix LIMIT {maxResults}";
                await using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@prefix", prefix);

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var resultKeyString = reader.GetString(0);
                    var resultKey = StorageKeyConvert.Deserialize<T>(resultKeyString);
                    results.Add(resultKey);
                }
                return results;
            });
        }
    }
}
