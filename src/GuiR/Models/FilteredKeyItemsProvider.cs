using GuiR.Models.Virtualization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuiR.Models
{
    public class FilteredKeyItemsProvider : IItemsProvider<string>
    {
        private readonly IEnumerable<string> _allKeys;
        private readonly string _filter;
        private readonly int _count;

        private FilteredKeyItemsProvider(IEnumerable<string> allKeys, string filter, int count)
        {
            _allKeys = allKeys;
            _filter = filter;
            _count = count;
        }

        public int FetchCount() => _count;

        public IList<string> FetchRange(int startIndex, int count) =>
            _allKeys.Where(x => x.StartsWith(_filter)).Skip(startIndex).Take(count).ToList();

        public static async ValueTask<FilteredKeyItemsProvider> CreateAsync(IEnumerable<string> allKeys, string filter)
        {
            int count = 0;

            await Task.Run(() =>
            {
                count = allKeys.Count(x => x.StartsWith(filter));
            });

            return new FilteredKeyItemsProvider(allKeys, filter, count);
        }
    }
}
