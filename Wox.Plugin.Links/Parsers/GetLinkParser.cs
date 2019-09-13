using System;
using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.Links.Services;

namespace Wox.Plugin.Links.Parsers {
    public class GetLinkParser : IParser {
        private readonly ILinkProcessService _linkProcess;
        private readonly IStorage _storage;
        private IClipboardService _clipboardService;

        public GetLinkParser(IStorage storage, ILinkProcessService linkProcess, IClipboardService clipboardService) {
            _clipboardService = clipboardService;
            _linkProcess = linkProcess;
            _storage = storage;
        }

        public bool TryParse(IQuery query, out List<Result> results) {
            results = new List<Result>();

            if (string.IsNullOrWhiteSpace(query.FirstSearch)) {
                return false;
            }

            var links = _storage.GetLinks().Where(x => x.Shortcut.MatchShortcut(query.FirstSearch)).ToArray();
            if (links.Length == 0) {
                return false;
            }

            results.AddRange(links.Select(link => {
                var args = query.Arguments.ToArray();
                return Create(link, args.FirstOrDefault());
            }));
            return true;
        }

        public ParserPriority Priority { get; } = ParserPriority.High;

        private Result Create(Link link, string arg) {
            var data = Format(link.Path, arg);

            if (link.Type == LinkType.ClipboardTemplate) {
                return new Result {
                    Title = $"[{link.Shortcut}] {link.Description}",
                    SubTitle = data.Replace(Environment.NewLine, " ↵ "),
                    IcoPath = @"icon.png",
                    Action = context => {
                        _clipboardService.SetText(data);
                        return true;
                    }
                };
            }

            var canOpenLink = CanOpenLink(data);
            var description = string.IsNullOrEmpty(link.Description) ? "" : FormatDescription(link.Description, arg);
            return new Result {
                Title = $"[{link.Shortcut}] {description}",
                SubTitle = FormatDescription(link.Path, arg),
                IcoPath = @"icon.png",
                Action = context => {
                    if (canOpenLink) {
                        _linkProcess.Open(data);
                    }

                    return canOpenLink;
                }
            };
        }

        private static string Format(string format, string arg) {
            if (string.IsNullOrWhiteSpace(arg) && format.Contains("@@")) {
                return format;
            }

            return format?.Replace("@@", arg);
        }

        private static string FormatDescription(string format, string arg) {
            if (string.IsNullOrWhiteSpace(arg)) {
                arg = "{Parameter is missing}";
            }

            return format?.Replace("@@", arg);
        }

        private static bool CanOpenLink(string url) {
            return !url.Contains("@@");
        }
    }
}