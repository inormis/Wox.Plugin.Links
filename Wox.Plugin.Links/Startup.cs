using Autofac;

namespace Wox.Plugin.Links {
    public class Startup {
        public static IContainer Container { get; private set; }

        public static void Initialize(IPluginContext pluginContext) {
            new Migration(pluginContext).Execute();
            
            var container = new ContainerBuilder();
            container.RegisterModule(new AutofacModule(pluginContext));
            Container = container.Build();
        }

        public static T Resolve<T>() {
            return Container.Resolve<T>();
        }
    }
}