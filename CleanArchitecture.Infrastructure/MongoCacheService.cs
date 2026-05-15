using CleanArchitecture.Application.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure
{
    public class MongoCacheService : ISharedCacheData
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoCacheService(IMongoDatabase database)
        {
            _collection = database.GetCollection<BsonDocument>("cache");
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);

            var doc = await _collection.Find(filter).FirstOrDefaultAsync(ct);

            if (doc == null)
                return default;

            if (doc.Contains("expiredAt") && doc["expiredAt"].ToUniversalTime() < DateTime.UtcNow)
            {
                // hết hạn → xóa luôn
                await _collection.DeleteOneAsync(filter, ct);
                return default;
            }

            var json = doc["data"].ToJson();
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T data, TimeSpan? ttl = null, CancellationToken ct = default)
        {
            var expireTime = DateTime.UtcNow.Add(ttl ?? TimeSpan.FromMinutes(5));

            var bsonData = data.ToBsonDocument();

            var doc = new BsonDocument
        {
            { "_id", key },
            { "data", bsonData },
            { "expiredAt", expireTime }
        };

            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);

            await _collection.ReplaceOneAsync(
                filter,
                doc,
                new ReplaceOptions { IsUpsert = true },
                ct
            );
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            if (key.EndsWith("*"))
            {
                // hỗ trợ xóa prefix (ví dụ product:*)
                var prefix = key.TrimEnd('*');
                var filter = Builders<BsonDocument>.Filter.Regex("_id", new BsonRegularExpression($"^{prefix}"));
                await _collection.DeleteManyAsync(filter, ct);
            }
            else
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                await _collection.DeleteOneAsync(filter, ct);
            }
        }
    }
}
