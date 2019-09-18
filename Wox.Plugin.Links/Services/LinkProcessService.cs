using System.Diagnostics;

namespace Wox.Plugin.Links.Services {
    public interface ILinkProcessService {
        bool Open(string path);
    }

    internal class LinkProcess : ILinkProcessService {
        public bool Open(string path) {
            try {
                Process.Start(path);
                return true;
            }
            catch {
                return false;
            }
        }
    }
}