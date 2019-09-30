using StackExchange.Redis;

namespace GuiR.Models
{
    public struct HashCollectionEntry
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public HashCollectionEntry(HashEntry entry)
        {
            Key = entry.Name;

            Value = entry.Value.ToString();
        }
    }
}