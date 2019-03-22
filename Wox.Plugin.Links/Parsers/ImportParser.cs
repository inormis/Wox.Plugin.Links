using System;
using System.Collections.Generic;
using System.IO;

namespace Wox.Plugin.Links.Parsers {
    public class ImportParser : BaseParser {
        private readonly IFileService _fileService;

        private readonly IStorage _storage;

        public ImportParser(IStorage storage, IFileService fileService) : base("link") {
            _fileService = fileService;
            _storage = storage;
        }

        protected override bool CustomParse(IQuery query) {
            return _fileService.Exists(query.SecondToEndSearch) &&
                   string.Compare(_fileService.GetExtension(query.SecondToEndSearch), ".json",
                       StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        protected override List<Result> Execute(IQuery query) {
            var results = new List<Result> {Create(query.SecondToEndSearch)};
            return results;
        }

        public override ParserPriority Priority { get; } = ParserPriority.High;

        private Result Create(string jsonPath) {
            return new Result {
                Title = $"Target configuration to '{Path.GetFileName(jsonPath)}' file",
                SubTitle = "",
                IcoPath = @"icon.png",
                Action = context => _storage.TargetNewPath(jsonPath)
            };
        }
    }
}