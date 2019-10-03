using StackExchange.Redis;

namespace GuiR.Models
{
    public struct SortedSetCollectionEntry
    {
        public string Value { get; set; }

        public double Score { get; set; }

        public SortedSetCollectionEntry(SortedSetEntry entry)
        {
            Value = entry.Element.ToString();

            Score = entry.Score;
        }
    }
}
