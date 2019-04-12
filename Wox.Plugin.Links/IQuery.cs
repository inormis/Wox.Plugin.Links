namespace Wox.Plugin.Links {
    public interface IQuery {
        string FirstSearch { get; }

        string[] Arguments { get; }

        string SecondToEndSearch { get; }
    }
}