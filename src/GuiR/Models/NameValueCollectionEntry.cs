using StackExchange.Redis;

namespace GuiR.Models
{
    public struct NameValueCollectionEntry
    {
        public string Name { get; }

        public string Value { get; }

        public NameValueCollectionEntry(NameValueEntry entry)
        {
            Name = entry.Name;

            Value = entry.Value;
        }
    }
}
