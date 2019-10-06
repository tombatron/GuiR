using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;

namespace GuiR.Controls.KeyDisplay
{
    public class HyperLogLogKey : StringKey
    {
        public HyperLogLogKey(string key) : base(key, ServiceLocator.GetService<HyperLogLogKeyViewModel>()) { }
    }
}
