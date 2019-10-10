using GuiR.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class SetKeyViewModel : ListKeyViewModel
    {
        public SetKeyViewModel(RedisProxy redis) : base(redis)
        {
        }

        protected override ValueTask<IEnumerable<string>> GetDataAsync(string key) =>
            _redis.GetSetValueAsync(key);
    }
}
