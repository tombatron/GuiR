using StackExchange.Redis;

namespace GuiR.Models
{
    public struct GeoPositionCollectionEntry
    {
        public string Name { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public GeoPositionCollectionEntry(string memberName, GeoPosition positionEntry)
        {
            Name = memberName;
            Latitude = positionEntry.Latitude;
            Longitude = positionEntry.Longitude;
        }
    }
}
