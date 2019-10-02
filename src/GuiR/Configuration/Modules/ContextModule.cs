using Autofac;
using GuiR.Redis;

namespace GuiR.Configuration.Modules
{
    class ContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultServerContext>().As<IServerContext>().SingleInstance();
        }
    }
}
