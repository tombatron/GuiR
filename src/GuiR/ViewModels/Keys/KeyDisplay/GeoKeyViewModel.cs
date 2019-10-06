using GuiR.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuiR.ViewModels.Keys.KeyDisplay
{
    public class GeoKeyViewModel : ListKeyViewModel
    {
        public GeoKeyViewModel(RedisProxy redis) : base(redis) { }

        protected override ValueTask<IEnumerable<string>> GetDataAsync(string key) =>
            _redis.GetGeoHashSetAsync(key);
    }
}
