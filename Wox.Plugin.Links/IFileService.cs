namespace Wox.Plugin.Links {
    public interface IFileService {
        void Open(string path);

        bool Exists(string filePath);

        void Start(string command, string args);
        bool WriteAllText(string filePath, string content);
        string ReadAllText(string path);
        string GetExtension(string filePath);
    }
}