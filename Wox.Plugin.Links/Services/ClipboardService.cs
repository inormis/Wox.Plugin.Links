using System;
using System.Threading;
using System.Windows.Forms;

namespace Wox.Plugin.Links.Services {
    public interface IClipboardService {
        void SetText(string text);
        string GetText();
    }

    public class ClipboardService : IClipboardService {
        public void SetText(string text) {
            Execute(() => Clipboard.SetText(text));
        }

        public string GetText() {
            var text = @"";
            Execute(() => text = Clipboard.GetText());
            return text;
        }

        private void Execute(Action action) {
            Thread staThread = new Thread(
                delegate() {
                    try {
                        action();
                    }
                    catch {
                    }
                });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}