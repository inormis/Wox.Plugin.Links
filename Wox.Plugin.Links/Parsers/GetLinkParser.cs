using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.Links.Services;

namespace Wox.Plugin.Links.Parsers {
    public class GetLinkParser : IParser {
        private readonly ILinkProcess _linkProcess;
        private readonly IStorage _storage;

        public GetLinkParser(IStorage storage, ILinkProcess linkProcess) {
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

        private Result Create(Link x, string arg) {
            var url = Format(x.Path, arg);
            var canOpenLink = CanOpenLink(url);
            var description = string.IsNullOrEmpty(x.Description) ? "" : FormatDescription(x.Description, arg);
            return new Result {
                Title = $"[{x.Shortcut}] {description}",
                SubTitle = FormatDescription(x.Path, arg),
                IcoPath = @"icon.png",
                Action = context => {
                    if (canOpenLink) {
                        _linkProcess.Open(url);
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