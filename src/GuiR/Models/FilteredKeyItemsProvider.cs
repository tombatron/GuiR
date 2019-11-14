using GuiR.Models.Virtualization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GuiR.Models.FileSystemBackedKeyCollection;

namespace GuiR.Models
{
    public class FilteredKeyItemsProvider : IItemsProvider<string>
    {
        private readonly ICacheFile _cacheFile;
        private readonly string _filter;
        private readonly List<int> _index;

        private FilteredKeyItemsProvider(ICacheFile cacheFile, string filter, List<int> index)
        {
            _cacheFile = cacheFile;
            _filter = filter;
            _index = index;
        }

        public int FetchCount() => _index.Count;

        public IList<string> FetchRange(int startIndex, int count)
        {
            var result = new List<string>();

            foreach (var i in _index.Skip(startIndex).Take(count))
            {
                result.Add(_cacheFile.ReadAtLine(i));
            }

            return result;
        }

        public static async ValueTask<FilteredKeyItemsProvider> CreateAsync(ICacheFile cacheFile, string filter)
        {
            var index = new List<int>();
            var i = 0;

            if (!string.IsNullOrEmpty(filter))
            {
                await Task.Run(() =>
                {
                    foreach (var key in cacheFile.ReadAllLines())
                    {
                        if (key.StartsWith(filter))
                        {
                            index.Add(i);
                        }

                        i++;
                    }
                });
            }

            return new FilteredKeyItemsProvider(cacheFile, filter, index);
        }
    }
}