using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace GuiR.Redis
{
    public sealed class KeyInfo : IDisposable
    {
        public IEnumerable<string> Keys { get; }

        public ConnectionMultiplexer ConnectionMultiplexer { get; }

        public int KeyCount { get; }

        public KeyInfo(IEnumerable<string> keys, ConnectionMultiplexer connectionMultiplexer, int keyCount)
        {
            Keys = keys;
            ConnectionMultiplexer = connectionMultiplexer;
            KeyCount = keyCount;
        }

        public void Dispose()
        {
            ConnectionMultiplexer.Dispose();
        }
    }
}
