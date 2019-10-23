using GuiR.Models.Virtualization;
using System.Collections.Generic;

namespace GuiR.Models
{
    public class EmptyKeyItemsProvider : IItemsProvider<string>
    {
        public int FetchCount() => 0;

        public IList<string> FetchRange(int startIndex, int count) => new List<string>();
    }
}
