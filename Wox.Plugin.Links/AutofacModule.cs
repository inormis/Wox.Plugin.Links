using System.Configuration;
using System.IO;
using Autofac;
using Serilog;
using Wox.Plugin.Links.Parsers;
using Wox.Plugin.Links.Services;

namespace Wox.Plugin.Links {
    public class AutofacModule : Module {
        private readonly IPluginContext _pluginContext;

        public AutofacModule(IPluginContext pluginContext) {
            _pluginContext = pluginContext;
        }

        protected override void Load(ContainerBuilder container) {
            base.Load(container);
            container.RegisterInstance(_pluginContext).As<IPluginContext>().SingleInstance();
            container.RegisterType<Engine>().AsImplementedInterfaces().SingleInstance();
            container.RegisterType<SettingsProvider>().AsImplementedInterfaces().SingleInstance();
            container.RegisterType<Storage>().As<IStorage>().SingleInstance();
            container.RegisterType<ClipboardService>().As<IClipboardService>().SingleInstance();
            container.RegisterType<LinkProcess>().As<ILinkProcessService>().SingleInstance();
            container.RegisterType<FileService>().As<IFileService>().SingleInstance();

            container.RegisterType<SaveParser>().As<IParser>().AsSelf().SingleInstance();
            container.RegisterType<ImportParser>().As<IParser>().AsSelf().SingleInstance();
            container.RegisterType<ExportParser>().As<IParser>().AsSelf().SingleInstance();
            container.RegisterType<RenameParser>().As<IParser>().AsSelf().SingleInstance();
            container.RegisterType<GetLinkParser>().As<IParser>().AsSelf().SingleInstance();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(_pluginContext.Directory, "logs\\Logs.txt"),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            container.RegisterInstance(logger).As<ILogger>();
        }
    }
}