namespace Wox.Plugin.Links {
    public interface IPluginContext {
        string Directory { get; }
    }

    public class PluginContext : IPluginContext {
        private readonly PluginInitContext _pluginInitContext;

        public PluginContext(PluginInitContext pluginInitContext) {
            _pluginInitContext = pluginInitContext;
        }

        public string Directory => _pluginInitContext.CurrentPluginMetadata.PluginDirectory;
    }
}