using Autofac;

namespace GuiR.Configuration
{
    public static class ServiceLocator
    {
        private static readonly IContainer _container;
        private static readonly ILifetimeScope _scope;

        static ServiceLocator()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(App).Assembly);

            _container = builder.Build();

            _scope = _container.BeginLifetimeScope();
        }

        public static TService GetService<TService>() =>
            _scope.Resolve<TService>();

        public static void Dispose()
        {
            _scope.Dispose();
            _container.Dispose();
        }
    }
}
