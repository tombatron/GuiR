using GuiR.Configuration;
using System.Windows;

namespace GuiR
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ServiceLocator.Dispose();
        }
    }
}
