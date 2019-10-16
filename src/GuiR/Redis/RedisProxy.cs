using GuiR.Models;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GuiR.Redis
{
    public class RedisProxy : IDisposable
    {
        private readonly ConcurrentDictionary<RedisServerInformation, ConnectionMultiplexer> _muxrs =
            new ConcurrentDictionary<RedisServerInformation, ConnectionMultiplexer>();

        private readonly IServerContext _serverContext;

        public RedisProxy(IServerContext serverContext) => _serverContext = serverContext;

        public ValueTask<IGrouping<string, KeyValuePair<string, string>>[]> GetInfoAsync() =>
            WithServer(async (server) =>
            {
                return await server.InfoAsync();
            });

        public ValueTask<string> GetSlowLogAsync() =>
            WithServer<string>(async (server) =>
            {
                var slowLogResult = await server.SlowlogGetAsync();

                return default;
            });

        public ValueTask<List<string>> GetKeysAsync(string filter) =>
            WithServer((server) =>
            {
                var result = new List<string>();

                foreach (var key in server.Keys(_serverContext.DatabaseId, pattern: filter))
                {
                    result.Add(key);
                }

                return Task.FromResult(result);
            });

        public ValueTask<string> GetStringValueAsync(string key) =>
            WithDatabase(async (db) => (string)await db.StringGetAsync(key));

        public ValueTask<IEnumerable<string>> GetListValueAsync(string key) =>
            WithDatabase(async (db) => (await db.ListRangeAsync(key)).Select(r => r.ToString()));

        public ValueTask<IEnumerable<string>> GetSetValueAsync(string key) =>
            WithDatabase(async (db) => (await db.SetMembersAsync(key)).Select(r => r.ToString()));

        public ValueTask<string> GetKeyTypeAsync(string key) =>
            WithDatabase(async (db) =>
            {
                var keyType = (await db.KeyTypeAsync(key)).ToString().ToLowerInvariant();

                if (keyType == RedisTypes.StringType)
                {
                    var stringPrefix = await db.StringGetRangeAsync(key, 0, 3);

                    if (stringPrefix == "HYLL")
                    {
                        return RedisTypes.HyperLogLog;
                    }
                    else
                    {
                        return RedisTypes.StringType;
                    }
                }
                else
                {
                    return keyType;
                }
            });

        public ValueTask<IEnumerable<HashCollectionEntry>> GetHashAsync(string key) =>
            WithDatabase(async (db) => (await db.HashGetAllAsync(key)).Select(r => new HashCollectionEntry(r)));

        public ValueTask<IEnumerable<SortedSetCollectionEntry>> GetSortedSetAsync(string key) =>
            WithDatabase(async (db) => (await db.SortedSetRangeByScoreWithScoresAsync(key)).Select(r => new SortedSetCollectionEntry(r)));

        public ValueTask<IEnumerable<GeoPositionCollectionEntry>> GetGeoHashSetAsync(string key) =>
            WithDatabase(async (db) =>
            {
                var members = await db.SortedSetRangeByRankAsync(key);

                var positions = await db.GeoPositionAsync(key, members);

                var zippedMemberPositions = Enumerable.Zip(members.Select(x => x.ToString()), positions.Select(x => x.Value), (string member, GeoPosition position) => (member, position));

                return zippedMemberPositions.Select(x => new GeoPositionCollectionEntry(x.member, x.position));
            });

        public ValueTask<long> GetHyperLogLogCountAsync(string key) =>
            WithDatabase(async (db) =>
            {
                return await db.HyperLogLogLengthAsync(key);
            });

        public ValueTask<IEnumerable<StreamCollectionEntry>> GetStreamDataAsync(string key, int count = 5) =>
            WithDatabase(async (db) =>
            {
                var entries = await db.StreamRangeAsync(key, count: count);

                return entries.Where(x => !x.IsNull).Select(x => new StreamCollectionEntry(x));
            });

        public ValueTask<IEnumerable<StreamCollectionEntry>> GetNextStreamDataAsync(string key, string minId, int count = 5) =>
            WithDatabase(async (db) =>
            {
                var entries = await db.StreamRangeAsync(key, minId: minId, count: count);

                return entries.Where(x => !x.IsNull).Select(x => new StreamCollectionEntry(x));
            });

        public ValueTask<IEnumerable<StreamCollectionEntry>> GetPreviousStreamDataAsync(string key, string maxId, int count = 5) =>
            WithDatabase(async (db) =>
            {
                var entries = await db.StreamRangeAsync(key, maxId: maxId, messageOrder: Order.Descending, count: count);

                return entries.Where(x => !x.IsNull).Select(x => new StreamCollectionEntry(x)).OrderBy(x => x.Id).Select(x => x);
            });

        private async ValueTask<ConnectionMultiplexer> GetConnectionMultiplexerAsync(RedisServerInformation serverInfo)
        {
            if (_muxrs.TryGetValue(serverInfo, out var existingMuxr))
            {
                return existingMuxr;
            }
            else
            {
                var muxr = await ConnectionMultiplexer.ConnectAsync(serverInfo);

                _muxrs.TryAdd(serverInfo, muxr);

                return muxr;
            }
        }

        private async ValueTask<TResult> WithDatabase<TResult>(Func<IDatabase, Task<TResult>> databaseAction)
        {
            try
            {
                var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
                var database = muxr.GetDatabase(_serverContext.DatabaseId);

                return await databaseAction(database);
            }
            catch (RedisConnectionException ex)
            {
                MessageBox.Show($"Error connecting to Redis. Exception: {ex.Message}");

                return default;
            }
        }

        private async ValueTask<TResult> WithServer<TResult>(Func<IServer, Task<TResult>> serverAction)
        {
            try
            {
                var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
                var firstEndpoint = muxr.GetEndPoints()[0];
                var server = muxr.GetServer(firstEndpoint);

                return await serverAction(server);
            }
            catch (RedisConnectionException ex)
            {
                MessageBox.Show($"Error connecting to Redis. Exception: {ex.Message}");

                return default;
            }
        }

        public void Dispose()
        {
            foreach (var muxr in _muxrs.Values)
            {
                muxr.Dispose();
            }
        }
    }
}
