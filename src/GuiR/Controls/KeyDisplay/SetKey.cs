using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;

namespace GuiR.Controls.KeyDisplay
{
    public class SetKey : ListKey
    {
        public SetKey(string key) : base(key, ServiceLocator.GetService<SetKeyViewModel>()) { }
    }
}
