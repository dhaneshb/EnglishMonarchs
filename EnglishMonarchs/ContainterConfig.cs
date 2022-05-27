using Autofac;

namespace EnglishMonarchs
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleManager>().As<IConsoleManager>();
            builder.RegisterType<MonarchProcessor>().As<IMonarchProcessor>();
            builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>();
            return builder.Build();
        }
    }
}
