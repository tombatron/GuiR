using GuiR.Models.Virtualization;
using System.Collections.Generic;
using System.Linq;

namespace GuiR.Models
{
    public class FilteredKeyItemsProvider : IItemsProvider<string>
    {
        private readonly IEnumerable<string> _allKeys;
        private readonly string _filter;
        private int _count = -1;

        public FilteredKeyItemsProvider(IEnumerable<string> allKeys, string filter)
        {
            _allKeys = allKeys;
            _filter = filter;
        }

        public int FetchCount()
        {
            if (_count < 0)
            {
                _count = _allKeys.Count(x => x.StartsWith(_filter));
            }

            return _count;
        }

        public IList<string> FetchRange(int startIndex, int count) =>
            _allKeys.Where(x => x.StartsWith(_filter)).Skip(startIndex).Take(count).ToList();
    }
}
