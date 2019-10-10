using StackExchange.Redis;
using System.Linq;

namespace GuiR.Models
{
    public struct StreamCollectionEntry
    {
        public string Id { get; }

        public NameValueCollectionEntry[] Values { get; }

        public StreamCollectionEntry(StreamEntry entry)
        {
            Id = entry.Id;

            Values = entry.Values.Select(x => new NameValueCollectionEntry(x)).ToArray();
        }
    }
}
