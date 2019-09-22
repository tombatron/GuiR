using Autofac;
using GuiR.Settings;

namespace GuiR.Configuration.Modules
{
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystemSettingsProvider>().As<ISettingsProvider>().SingleInstance();
        }
    }
}
