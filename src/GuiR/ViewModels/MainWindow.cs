using GuiR.Redis;

namespace GuiR.ViewModels
{
    public class MainWindow
    {
        private readonly RedisProxy _redis;

        public MainWindow(RedisProxy redis) => _redis = redis;
    }
}
