using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Wox.Plugin.Links.Services {
    public interface IClipboardService {
        void SetText(string text);

        string GetText();
    }

    public class ClipboardService : IClipboardService {
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static void PasteToApplication() {
            var handle = GetForegroundWindow();
            if (handle == IntPtr.Zero) {
                return;
            }
            SetForegroundWindow(handle);
            SendKeys.SendWait("^v");
        }

        public void SetText(string text) {
            Execute(() => PasteText(text), false);
        }

        private static void PasteText(string text) {
            Clipboard.SetText(text);
            Thread.Sleep(150);
            PasteToApplication();
        }

        public string GetText() {
            var text = @"";
            Execute(() => text = Clipboard.GetText(), true);
            return text;
        }

        private void Execute(Action action, bool wait) {
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
            if (wait)
                staThread.Join();
        }
    }
}