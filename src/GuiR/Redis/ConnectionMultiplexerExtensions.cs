using StackExchange.Redis;

namespace GuiR.Redis
{
    public static class ConnectionMultiplexerExtensions
    {
        public static IServer GetFirstServer(this ConnectionMultiplexer @this)
        {
            var endPoint = @this.GetEndPoints()[0];

            return @this.GetServer(endPoint);
        }
    }
}