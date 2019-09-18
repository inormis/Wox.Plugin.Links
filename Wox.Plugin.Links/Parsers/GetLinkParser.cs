using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Wox.Plugin.Links.Services;

namespace Wox.Plugin.Links.Parsers {
    public class GetLinkParser : IParser {
        private readonly ILinkProcessService _linkProcess;
        private readonly IStorage _storage;
        private readonly IClipboardService _clipboardService;

        public GetLinkParser(IStorage storage, ILinkProcessService linkProcess, IClipboardService clipboardService) {
            _clipboardService = clipboardService;
            _linkProcess = linkProcess;
            _storage = storage;
        }

        public bool TryParse(IQuery query, out List<Result> results) {
            results = new List<Result>();

            if (string.IsNullOrWhiteSpace(query.FirstSearch)) return false;

            var links = _storage.GetLinks().Where(x => x.Shortcut.MatchShortcut(query.FirstSearch)).ToArray();
            if (links.Length == 0) return false;

            results.AddRange(links.Select(link => {
                var args = query.Arguments.ToArray();
                return Create(link, args.FirstOrDefault());
            }));
            return true;
        }

        public ParserPriority Priority { get; } = ParserPriority.High;

        private Result Create(Link link, string arg) {
            var description = string.IsNullOrEmpty(link.Description) ? "" : FormatDescription(link.Description, arg);
            var title = $"[{link.Shortcut}] {description}";
            var formattedData = Format(link.Path, arg);
            var subTitle = link.Type == LinkType.Path
                ? formattedData
                : formattedData.Replace(Environment.NewLine, " ↵ ");

            return new Result {
                Title = title,
                SubTitle = subTitle,
                IcoPath = @"icon.png",
                Action = context => {
                    if (context.SpecialKeyState.CtrlPressed) {
                        var dialogResult = MessageBox.Show($"Delete shortcut: '{link.Shortcut}'?", "Confirmation",
                            MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes) _storage.Delete(link.Shortcut);
                        return true;
                    }

                    if (context.SpecialKeyState.AltPressed || link.Type == LinkType.ClipboardTemplate) {
                        _clipboardService.SetText(formattedData);
                        return true;
                    }

                    var canOpenLink = !formattedData.Contains("@@");
                    return canOpenLink && _linkProcess.Open(formattedData);
                }
            };
        }

        private static string Format(string format, string arg) {
            if (string.IsNullOrWhiteSpace(arg) && format.Contains("@@")) return format;

            return format?.Replace("@@", arg);
        }

        private static string FormatDescription(string format, string arg) {
            if (string.IsNullOrWhiteSpace(arg)) arg = "{Parameter is missing}";

            return format?.Replace("@@", arg);
        }
    }
}