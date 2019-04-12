using System.Diagnostics;

namespace Wox.Plugin.Links.Services {
    public interface ILinkProcessService {
        void Open(string path);
    }

    internal class LinkProcess : ILinkProcessService {
        public void Open(string path) {
            Process.Start(path);
        }
    }
}