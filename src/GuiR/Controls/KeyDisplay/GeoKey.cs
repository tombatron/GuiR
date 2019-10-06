using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;

namespace GuiR.Controls.KeyDisplay
{
    public class GeoKey : ListKey
    {
        public GeoKey(string key) : base(key, ServiceLocator.GetService<GeoKeyViewModel>()) { }
    }
}
