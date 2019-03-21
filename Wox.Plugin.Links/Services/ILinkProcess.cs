using System.Diagnostics;

namespace Wox.Plugin.Links.Services {
    public interface ILinkProcess {
        void Open(string path);
    }

    internal class LinkProcess : ILinkProcess {
        public void Open(string path) {
            Process.Start(path);
        }
    }
}