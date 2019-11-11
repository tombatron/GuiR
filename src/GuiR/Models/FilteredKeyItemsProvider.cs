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
        private readonly List<int> _index;

        private FilteredKeyItemsProvider(IEnumerable<string> allKeys, string filter, List<int> index)
        {
            _allKeys = allKeys;
            _filter = filter;
            _index = index;
        }

        public int FetchCount() => _index.Count;

        public IList<string> FetchRange(int startIndex, int count)
        {
            var rangeIndex = _index.Skip(startIndex).Take(count);
            var range = new List<string>();
            var i = 0;

            IEnumerable<string> currentEnumerable = _allKeys;

            foreach (var ri in rangeIndex)
            {
                currentEnumerable = currentEnumerable.Skip(ri - i);

                range.Add(currentEnumerable.Take(1).First());

                i = ri;
            }

            return range;
        }

        public static async ValueTask<FilteredKeyItemsProvider> CreateAsync(IEnumerable<string> allKeys, string filter)
        {
            var index = new List<int>();
            var i = 0;

            if (!string.IsNullOrEmpty(filter))
            {
                await Task.Run(() =>
                {
                    foreach (var key in allKeys)
                    {
                        if (key.StartsWith(filter))
                        {
                            index.Add(i);
                        }

                        i++;
                    }
                });
            }

            return new FilteredKeyItemsProvider(allKeys, filter, index);
        }
    }
}