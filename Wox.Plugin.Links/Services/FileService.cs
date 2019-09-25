using System;
using System.IO;

namespace Wox.Plugin.Links.Services {
    public interface IFileService {
        bool FileExists(string filePath);

        bool DirectoryExists(string path);

        bool WriteAllText(string filePath, string content);

        string ReadAllText(string path);

        string GetExtension(string filePath);

        void EnsureDirectoryExists(string path);
    }
    
    public class FileService : IFileService {
        public bool FileExists(string filePath) {
            return File.Exists(filePath);
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

        public void EnsureDirectoryExists(string path) {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }
    }
}