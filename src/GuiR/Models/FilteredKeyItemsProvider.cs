using GuiR.Models.Virtualization;
using System.Collections.Generic;

namespace GuiR.Models
{
    public class FilteredKeyItemsProvider : IItemsProvider<string>
    {
        private List<string> _filteredKeys;

        public FilteredKeyItemsProvider(List<string> filteredKeys) => _filteredKeys = filteredKeys;

        public int FetchCount() => _filteredKeys.Count;

        public IList<string> FetchRange(int startIndex, int count)
        {
            int returnCount = (count + startIndex) > _filteredKeys.Count ? (_filteredKeys.Count - startIndex) : count;

            return _filteredKeys.GetRange(startIndex, returnCount);
        }
    }
}
