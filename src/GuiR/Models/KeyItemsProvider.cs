using GuiR.Models.Virtualization;
using System.Collections.Generic;

namespace GuiR.Models
{
    public class KeyItemsProvider : IItemsProvider<string>
    {
        private FileSystemBackedKeyCollection _keys;

        public KeyItemsProvider(FileSystemBackedKeyCollection keys) => _keys = keys;

        public int FetchCount() => _keys.Count;

        public IList<string> FetchRange(int startIndex, int count) => _keys.FetchRange(startIndex, count);
    }
}
