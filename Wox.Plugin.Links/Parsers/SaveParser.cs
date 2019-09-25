using System;
using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.Links.Services;

namespace Wox.Plugin.Links.Parsers {
    public class SaveParser : BaseParser {
        private readonly IStorage _storage;
        private readonly IFileService _fileService;
        private readonly IClipboardService _clipboardService;

        public SaveParser(IStorage storage, IFileService fileService, IClipboardService clipboardService) :
            base(PluginKey) {
            _clipboardService = clipboardService;
            _fileService = fileService;
            _storage = storage;
        }

        protected override bool CustomParse(IQuery query) {
            return query.Arguments.Length >= 2;
        }

        protected override List<Result> Execute(IQuery query) {
            var querySearch = query.SecondToEndSearch;
            var linkType = GetLinkType(ref querySearch);

            var shortcut = querySearch.Split(' ').First().Trim();
            var rest = querySearch.Substring(shortcut.Length).Trim();


            if (linkType == LinkType.ClipboardTemplate)
                return new List<Result> {
                    CreateTemplateLink(shortcut, rest)
                };
            var descriptionSeparatorIndex = rest.IndexOf("|", StringComparison.InvariantCulture);
            var linkPath = descriptionSeparatorIndex == -1 ? rest : rest.Substring(0, descriptionSeparatorIndex).Trim();
            var description = descriptionSeparatorIndex == -1 ? "" : rest.Substring(descriptionSeparatorIndex + 1);

            return new List<Result> {
                CreatePathLinkResult(shortcut, linkType, linkPath.Trim(), description.Trim())
            };
        }

        private Result CreateTemplateLink(string shortCut, string description) {
            return new Result {
                Title = $"Save template '{shortCut}'",
                SubTitle = _clipboardService.GetText()?.Replace(Environment.NewLine, " ↵ "),
                IcoPath = @"icon.png",
                Action = context => {
                    var text = _clipboardService.GetText();
                    _storage.Set(shortCut, LinkType.ClipboardTemplate, text, description);
                    return true;
                }
            };
        }

        private LinkType GetLinkType(ref string rest) {
            var flag = rest.Split(' ').FirstOrDefault()?.ToLowerInvariant();
            if (flag == "-t") {
                rest = rest.Substring(flag.Length).Trim();
                return LinkType.ClipboardTemplate;
            }

            return LinkType.Path;
        }

        private Result CreatePathLinkResult(string shortCut, LinkType linkType, string linkPath, string description) {
            var isValidPath = _fileService.FileExists(linkPath) || _fileService.DirectoryExists(linkPath) ||
                              Uri.IsWellFormedUriString(linkPath, UriKind.Absolute);

            if (isValidPath) {
                return new Result {
                    Title = $"Save the link as \'{shortCut}\': \'{description}\'",
                    SubTitle = linkPath.Replace(Environment.NewLine, "↵"),
                    IcoPath = @"icon.png",
                    Action = context => {
                        _storage.Set(shortCut, LinkType.Path, linkPath, description);
                        return true;
                    }
                };
            }

            return new Result {
                Title =
                    $"'{linkPath}' is not a valid path for the link (Directories, Files, Abosiule url ares supported",
                IcoPath = @"icon.png",
                Action = context => false
            };
        }
    }
}