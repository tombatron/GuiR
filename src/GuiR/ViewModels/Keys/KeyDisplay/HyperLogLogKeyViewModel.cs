using GuiR.Redis;
using System.Threading.Tasks;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class HyperLogLogKeyViewModel : StringKeyViewModel
    {
        public HyperLogLogKeyViewModel(RedisProxy redis) : base(redis)
        {
        }

        protected override async ValueTask<string> GetDataAsync(string key)
        {
            var count = await _redis.GetHyperLogLogCountAsync(key);

            return count.ToString();
        }
    }
}
