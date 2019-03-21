using System.Collections.Generic;
using Autofac;
using NSubstitute;
using Wox.Plugin;
using Wox.Plugin.Links;
using Wox.Plugin.Links.Parsers;
using Xunit;

namespace Wox.Links.Tests {
    public class StartupTests {
        [Fact]
        public void Setup() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule(new PluginContext(new PluginInitContext())));
            var container = builder.Build();

            var context = Substitute.For<IPluginContext>();
            context.Directory.Returns(@"C:\PluginDirectory");
            Startup.Initialize(context);

            Startup.Resolve<IEnumerable<IParser>>();
            Startup.Resolve<IEngine>();
        }
    }
}