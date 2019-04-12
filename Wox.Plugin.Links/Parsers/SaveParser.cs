using System;
using System.Collections.Generic;
using System.Linq;

namespace Wox.Plugin.Links.Parsers {
    public class SaveParser : BaseParser {
        private readonly IStorage _storage;
        private readonly IFileService _fileService;

        public SaveParser(IStorage storage, IFileService fileService) : base(PluginKey) {
            _fileService = fileService;
            _storage = storage;
        }

        protected override bool CustomParse(IQuery query) {
            return query.Arguments.Length >= 2;
        }

        protected override List<Result> Execute(IQuery query) {
            var shortcut = query.SecondToEndSearch.Split(' ').FirstOrDefault();
            var rest = query.SecondToEndSearch.Substring(shortcut.Length).Trim();
            var args = rest.Split('|');
            var linkPath = args.FirstOrDefault().Trim();
            var description = args.Skip(1).FirstOrDefault()?.Trim();
            return new List<Result> {
                CreateResult(shortcut, linkPath, description)
            };
        }

        private Result CreateResult(string shortCut, string linkPath, string description) {
            var isValidPath = _fileService.FileExists(linkPath) || _fileService.DirectoryExists(linkPath) ||
                              Uri.IsWellFormedUriString(linkPath, UriKind.Absolute);

            if (isValidPath) {
                return new Result {
                    Title = $"Save the link as \'{shortCut}\': \'{description}\'",
                    SubTitle = linkPath,
                    IcoPath = @"icon.png",
                    Action = context => {
                        _storage.Set(shortCut, linkPath, description);
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