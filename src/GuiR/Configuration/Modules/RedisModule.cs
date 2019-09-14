using Autofac;
using GuiR.Redis;

namespace GuiR.Configuration.Modules
{
    public class RedisModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RedisProxy>().SingleInstance();
        }
    }
}
