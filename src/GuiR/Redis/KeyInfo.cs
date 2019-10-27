using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace GuiR.Redis
{
    public sealed class KeyInfo : IDisposable
    {
        public IEnumerable<string> Keys { get; }

        public ConnectionMultiplexer ConnectionMultiplexer { get; }

        public long KeyCount { get; }

        public KeyInfo(IEnumerable<string> keys, ConnectionMultiplexer connectionMultiplexer, long keyCount)
        {
            Keys = keys;
            ConnectionMultiplexer = connectionMultiplexer;
            KeyCount = keyCount;
        }

        public void Dispose()
        {
            if (ConnectionMultiplexer != null)
            {
                ConnectionMultiplexer.Dispose();
            }
        }
    }
}
