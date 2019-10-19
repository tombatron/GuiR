using GuiR.Models.Virtualization;
using System.Collections.Generic;
using System.Linq;

namespace GuiR.Models
{
    public class KeyItemsProvider : IItemsProvider<string>
    {
        private IEnumerable<string> _keyEnumerable;

        public KeyItemsProvider(IEnumerable<string> keyEnumerable) => _keyEnumerable = keyEnumerable;

        public int FetchCount()
        {
            return 1_000_000;
        }

        public IList<string> FetchRange(int startIndex, int count)
        {
            _keyEnumerable = _keyEnumerable.Skip(startIndex);

            return _keyEnumerable.Take(count).ToList();
        }
    }
}
