namespace Wox.Plugin.Links {
    public interface IFileService {
        bool FileExists(string filePath);

        bool DirectoryExists(string path);

        bool WriteAllText(string filePath, string content);

        string ReadAllText(string path);

        string GetExtension(string filePath);
    }
}