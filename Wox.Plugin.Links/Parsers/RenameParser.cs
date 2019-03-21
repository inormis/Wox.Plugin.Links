using System;
using System.Collections.Generic;
using System.Linq;

namespace Wox.Plugin.Links.Parsers {
    public class RenameParser : BaseParser {
        private readonly IStorage _storage;

        public RenameParser(IStorage storage) : base("rename") {
            _storage = storage;
        }

        protected override List<Result> Execute(IQuery query) {
            if (query.Arguments.Length == 0) {
                return GetErrorResult("Pass the new name of shortcut");
            }

            var newName = query.Arguments.First();

            if (query.Arguments.Length == 1) {
                return GetErrorResult("Pass a name of the link to be updated");
            }

            if (query.Arguments.Length > 2) {
                return GetErrorResult("One or two arguments have to be specified argument has to be specified");
            }

            if (_storage.TryGetByShortcut(newName, out var existingLink)) {
                return GetErrorResult($"Shortcut [{newName}] already exists", existingLink.Description);
            }

            var existingShortCut = query.Arguments[1];

            var predicate = string.IsNullOrWhiteSpace(existingShortCut)
                ? (Func<string, bool>) (s => true)
                : s => s.ContainsCaseInsensitive(existingShortCut);


            var results = _storage.GetLinks()
                .Where(link => predicate(link.Shortcut))
                .Select(x => new Result {
                    Title = $"Rename shortcut [{x.Shortcut}] to => [{newName}]",
                    SubTitle = $"Description: {x.Description}",
                    Action = context => {
                        _storage.Rename(x.Shortcut, newName);
                        return true;
                    }
                })
                .OrderBy(x => x.Title)
                .ToList();
            return results.Any()
                ? results
                : GetErrorResult("No shortcuts were found matching your query");
        }
    }
}