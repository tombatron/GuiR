namespace GuiR.Redis
{
    public struct DatabaseInfo
    {
        public short Id { get; }
        public long KeyCount { get; }

        public DatabaseInfo(short id, long keyCount)
        {
            Id = id;
            KeyCount = keyCount;
        }
    }
}