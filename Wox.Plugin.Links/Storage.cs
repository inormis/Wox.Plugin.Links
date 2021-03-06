﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog;
using Wox.Plugin.Links.Services;

namespace Wox.Plugin.Links {
    public interface IStorage {
        void Set(string shortcut, LinkType linkType, string data, string description);

        Link[] GetLinks();

        void Delete(string shortcut);

        bool TargetNewPath(string jsonPath);

        string ExportAsJsonString();

        bool TryGetByShortcut(string shortcut, out Link link);

        void Rename(string existingShortCut, string newShortcut);
    }

    public class Storage : IStorage {
        private readonly Configuration _configuration;
        private Dictionary<string, Link> _links;
        private readonly IFileService _fileService;

        public Storage(IFileService fileService, ILogger logger) {
            _logger = logger;
            _fileService = fileService;

            var appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Wox.Plugins.Links");
            _fileService.EnsureDirectoryExists(appDirectory);
            _configurationPath = Path.Combine(appDirectory, "config.json");
            _defaultLinksPath = Path.Combine(appDirectory, "links.json");
            _logger.Information(
                $"Initiate {nameof(Storage)}, ConfigurationPath: '{_configurationPath}', DefaultLinksPath: '{_defaultLinksPath}'");
            _configuration = LoadConfiguration();
            _links = LoadLinks();
        }

        private readonly string _configurationPath;
        private readonly string _defaultLinksPath;
        private readonly ILogger _logger;

        public void Set(string shortcut, LinkType linkType, string data, string description) {
            _links[shortcut] = new Link {
                Path = data,
                Shortcut = shortcut,
                Description = description,
                Type = linkType
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
            if (_fileService.FileExists(_configurationPath)) {
                var content = _fileService.ReadAllText(_configurationPath);
                _logger.Information($"Loaded configuration: {content}");
                return JsonConvert.DeserializeObject<Configuration>(content);
            }

            var configuration = new Configuration {
                LinksFilePath = _defaultLinksPath
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
            _logger.Information($"Loaded [{links.Length}] links from {linksFilePath}");
            return links.ToDictionary(x => x.Shortcut, x => x);
        }

        private void Save() {
            var serializedConfiguration = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
            _fileService.WriteAllText(_configurationPath, serializedConfiguration);

            var content = JsonConvert.SerializeObject(_links.Values.ToArray(), Formatting.Indented);
            _fileService.WriteAllText(_configuration.LinksFilePath, content);

            _logger.Information($"Saved [{_links}] links to {_configuration.LinksFilePath}");
        }
    }

    public class Configuration {
        public int Version { get; set; } = 1;

        public string LinksFilePath { get; set; }
    }
}