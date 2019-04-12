using System;
using System.Diagnostics;
using System.IO;

namespace Wox.Plugin.Links {
    public class FileService : IFileService {
        public void Open(string path) {
            Process.Start(path);
        }

        public bool FileExists(string filePath) {
            return File.Exists(filePath);
        }

        public void Start(string command, string args) {
            Process.Start(command, args);
        }

        public bool WriteAllText(string filePath, string content) {
            try {
                File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        public string GetExtension(string filePath) {
            return Path.GetExtension(filePath);
        }

        public bool DirectoryExists(string path) {
            return Directory.Exists(path);
        }

        public string ReadAllText(string path) {
            return File.ReadAllText(path);
        }
    }
}