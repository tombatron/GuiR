﻿using GuiR.Models;
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

        private readonly IServerContext _serverContext;

        public RedisProxy(IServerContext serverContext) => _serverContext = serverContext;

        public async ValueTask<string> GetInfoAsync()
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
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

        public async ValueTask<string> GetSlowLogAsync()
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
            var firstEndpoint = muxr.GetEndPoints()[0];
            var server = muxr.GetServer(firstEndpoint);

            var slowLogResult = await server.SlowlogGetAsync();

            return default;
        }

        public async ValueTask<List<string>> GetKeysAsync(string filter)
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
            var firstEndpoint = muxr.GetEndPoints().First();
            var server = muxr.GetServer(firstEndpoint);

            var result = new List<string>();

            foreach (var key in server.Keys(_serverContext.DatabaseId, pattern: filter))
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

        public async ValueTask<string> GetStringValueAsync(string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
            var database = muxr.GetDatabase(_serverContext.DatabaseId);

            return await database.StringGetAsync(key);
        }

        public async ValueTask<IEnumerable<string>> GetListValueAsync(string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
            var database = muxr.GetDatabase(_serverContext.DatabaseId);

            var result = await database.ListRangeAsync(key);

            return result.Select(r => r.ToString());
        }

        public async ValueTask<IEnumerable<string>> GetSetValueAsync(string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
            var database = muxr.GetDatabase(_serverContext.DatabaseId);

            var result = await database.SetMembersAsync(key);

            return result.Select(r => r.ToString());
        }

        public async ValueTask<string> GetKeyTypeAsync(string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
            var database = muxr.GetDatabase(_serverContext.DatabaseId);

            return (await database.KeyTypeAsync(key)).ToString().ToLowerInvariant();
        }

        public async ValueTask<IEnumerable<HashCollectionEntry>> GetHashAsync(string key)
        {
            var muxr = await GetConnectionMultiplexerAsync(_serverContext.ServerInfo);
            var database = muxr.GetDatabase(_serverContext.DatabaseId);

            var result = await database.HashGetAllAsync(key);

            return result.Select(r => new HashCollectionEntry(r));
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
