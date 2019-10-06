using GuiR.Redis;
using System.Threading.Tasks;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class HyperLogLogKeyViewModel : StringKeyViewModel
    {
        public HyperLogLogKeyViewModel(RedisProxy redis) : base(redis)
        {
        }

        protected override ValueTask<string> GetDataAsync(string key)
        {
            return base.GetDataAsync(key);
        }
    }
}
