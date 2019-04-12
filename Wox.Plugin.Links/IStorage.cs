using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Wox.Plugin.Links {
    public interface IStorage {
        void Set(string shortcut, string url, string description);

        Link[] GetLinks();

        void Delete(string shortcut);
        bool TargetNewPath(string jsonPath);
        string ExportAsJsonString();
        bool TryGetByShortcut(string shortcut, out Link link);
        void Rename(string existingShortCut, string newShortcut);
    }

    internal class Storage : IStorage {
        private readonly Configuration _configuration;
        private readonly string _directory;
        private Dictionary<string, Link> _links;
        private readonly IFileService _fileService;

        public Storage(IPluginContext pluginContext, IFileService fileService) {
            _fileService = fileService;
            _directory = pluginContext.Directory;
            _configuration = LoadConfiguration();
            _links = LoadLinks();
        }

        private string ConfigurationPath => Path.Combine(_directory ?? "", "config.json");
        private string DefaultLinksPath => Path.Combine(_directory ?? "", "links.json");

        public void Set(string shortcut, string url, string description) {
            _links[shortcut] = new Link {
                Path = url,
                Shortcut = shortcut,
                Description = description
            };
            Save();
        }

        public Link[] GetLinks() {
            return _links.Values.ToArray();
        }

        public void Delete(string shortcut) {
            _links.Remove(shortcut);
            Save();
        }

        public bool TargetNewPath(string jsonPath) {
            try {
                _links = ReadLinksFromFile(jsonPath);
                _configuration.LinksFilePath = jsonPath;
                Save();
                return true;
            }
            catch {
                return false;
            }
        }

        public string ExportAsJsonString() {
            return JsonConvert.SerializeObject(_links, Formatting.Indented);
        }

        public bool TryGetByShortcut(string shortcut, out Link link) {
            return _links.TryGetValue(shortcut, out link);
        }

        public void Rename(string existingShortCut, string newShortcut) {
            var link = _links[existingShortCut];
            link.Shortcut = newShortcut;
            _links.Add(newShortcut, link);
            _links.Remove(existingShortCut);

            Save();
        }

        private Configuration LoadConfiguration() {
            if (_fileService.FileExists(ConfigurationPath)) {
                return JsonConvert.DeserializeObject<Configuration>(_fileService.ReadAllText(ConfigurationPath));
            }

            var configuration = new Configuration {
                LinksFilePath = DefaultLinksPath
            };
            return configuration;
        }

        private Dictionary<string, Link> LoadLinks() {
            var linksFilePath = _configuration.LinksFilePath;
            if (_fileService.FileExists(linksFilePath)) {
                return ReadLinksFromFile(linksFilePath);
            }

            return new Dictionary<string, Link>();
        }

        private Dictionary<string, Link> ReadLinksFromFile(string linksFilePath) {
            var links = JsonConvert.DeserializeObject<Link[]>(_fileService.ReadAllText(linksFilePath));
            return links.ToDictionary(x => x.Shortcut, x => x);
        }

        private void Save() {
            var serializedConfiguration = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
            _fileService.WriteAllText(ConfigurationPath, serializedConfiguration);

            var content = JsonConvert.SerializeObject(_links.Values.ToArray(), Formatting.Indented);
            _fileService.WriteAllText(_configuration.LinksFilePath, content);
        }
    }

    public class Configuration {
        public string LinksFilePath { get; set; }
    }
}