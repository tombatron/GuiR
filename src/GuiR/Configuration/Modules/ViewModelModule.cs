using Autofac;
using GuiR.ViewModels.Info;
using GuiR.ViewModels.Keys;
using GuiR.ViewModels.Keys.KeyDisplay;

namespace GuiR.Configuration.Modules
{
    public class ViewModelModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ViewModels.MainWindow>();

            builder.RegisterType<InfoControlViewModel>();
            builder.RegisterType<KeysControlViewModel>();

            builder.RegisterType<StringKeyViewModel>();
        }
    }
}