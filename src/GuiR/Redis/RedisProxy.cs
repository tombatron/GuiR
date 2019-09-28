using GuiR.Models;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiR.Redis
{
    public class RedisProxy : IDisposable
    {
        private readonly ConcurrentDictionary<RedisServerInformation, ConnectionMultiplexer> _muxrs =
            new ConcurrentDictionary<RedisServerInformation, ConnectionMultiplexer>();

        public async ValueTask<string> GetInfoAsync(RedisServerInformation serverInfo)
        {
            var muxr = await GetConnectionMultiplexerAsync(serverInfo);
            var firstEndpoint = muxr.GetEndPoints()[0];
            var server = muxr.GetServer(firstEndpoint);

            var infoResult = await server.InfoAsync();

            var result = new StringBuilder();

            foreach (var grouping in infoResult)
            {
                result.AppendLine(grouping.Key);

                foreach (var item in grouping)
                {
                    result.AppendLine($"{item.Key} | {item.Value}");
                }
            }

            return result.ToString();
        }

        public async ValueTask<string> GetSlowLogAsync(RedisServerInformation serverInfo)
        {
            var muxr = await GetConnectionMultiplexerAsync(serverInfo);
            var firstEndpoint = muxr.GetEndPoints()[0];
            var server = muxr.GetServer(firstEndpoint);

            var slowLogResult = await server.SlowlogGetAsync();

            return default;
        }

        public async ValueTask<List<string>> GetKeysAsync(RedisServerInformation serverInfo, int databaseId, string filter)
        {
            var muxr = await GetConnectionMultiplexerAsync(serverInfo);
            var firstEndpoint = muxr.GetEndPoints().First();
            var server = muxr.GetServer(firstEndpoint);

            var result = new List<string>();

            foreach (var key in server.Keys(databaseId, pattern: filter))
            {
                result.Add(key);
            }

            return result;
        }

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

        public async ValueTask<string> GetStringValueAsync(RedisServerInformation serverInfo, int databaseId, string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(serverInfo);
            var database = muxr.GetDatabase(databaseId);

            return await database.StringGetAsync(key);
        }

        public async ValueTask<IEnumerable<string>> GetListValueAsync(RedisServerInformation serverInfo, int databaseId, string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(serverInfo);
            var database = muxr.GetDatabase(databaseId);

            var result = await database.ListRangeAsync(key);

            return result.Select(r => r.ToString());
        }

        public async ValueTask<string> GetKeyTypeAsync(RedisServerInformation serverInfo, int databaseId, string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(serverInfo);
            var database = muxr.GetDatabase(databaseId);

            return (await database.KeyTypeAsync(key)).ToString().ToLowerInvariant();
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
