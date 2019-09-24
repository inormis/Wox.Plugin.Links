using System;
using System.IO;
using System.Linq;

namespace Wox.Plugin.Links {
    public class Migration {
        private readonly IPluginContext _pluginContext;
        private readonly string _appDirectory;

        public Migration(IPluginContext pluginContext) {
            _pluginContext = pluginContext;
            _appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Wox.Plugins.Links");

            if (!Directory.Exists(_appDirectory))
                Directory.CreateDirectory(_appDirectory);
        }

        public void Execute() {
            MoveSettingsToAppData();
        }

        private void MoveSettingsToAppData() {
            var destFileName = Path.Combine(_appDirectory, "links.json");
            if (File.Exists(destFileName))
                return;
            
            

            var oldDirectory = Directory.GetParent(_pluginContext.Directory).FullName;
            var lastLinksFile = Directory.GetFiles(oldDirectory, "*.json", SearchOption.AllDirectories)
                .Where(x => Path.GetFileName(x).ToLowerInvariant() == "links.json")
                .OrderByDescending(x => new FileInfo(x).LastWriteTime)
                .FirstOrDefault();
            
            if (lastLinksFile != null) {
                File.Move(lastLinksFile, destFileName);
            }
        }
    }
}