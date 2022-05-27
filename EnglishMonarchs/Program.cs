using Autofac;

namespace EnglishMonarchs
{
    class Program
    {
        static void Main(string[] args)
        {
            //Implemented dependency injection to decrease the usage of dependencies.
            var container = ContainerConfig.Configure();
            using var scope  = container.BeginLifetimeScope();
            var app = scope.Resolve<IMonarchProcessor>();
            app.Run();
        }
    }
}
