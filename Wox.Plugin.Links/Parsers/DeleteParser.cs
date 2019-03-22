using System;
using System.Collections.Generic;
using System.Linq;

namespace Wox.Plugin.Links.Parsers {
    public class DeleteParser : BaseParser {
        private readonly IStorage _storage;

        public DeleteParser(IStorage storage) : base("delete") {
            _storage = storage;
        }

        protected override List<Result> Execute(IQuery query) {
            if (query.Arguments.Length >= 2) {
                return GetErrorResult("Only one argument has to be specified");
            }

            var arg = query.Arguments.SingleOrDefault();

            var predicate = string.IsNullOrWhiteSpace(arg)
                ? (Func<string, bool>) (s => true)
                : s => s.ContainsCaseInsensitive(arg);
            return _storage.GetLinks()
                .Where(link => predicate(link.Shortcut))
                .Select(x => new Result {
                    Title = $"Delete '{x.Shortcut}' link",
                    SubTitle = x.Path,
                    IcoPath = @"icon.png",
                    Action = context => {
                        _storage.Delete(x.Shortcut);
                        return true;
                    }
                })
                .OrderBy(x => x.Title)
                .ToList();
        }
    }
}