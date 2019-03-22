using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Wox.Plugin.Links.Parsers {
    public class SaveParser : BaseParser {
        private readonly IStorage _storage;

        public SaveParser(IStorage storage) : base(PluginKey) {
            _storage = storage;
        }

        protected override bool CustomParse(IQuery query) {
            return query.Arguments.Length >= 2;
        }

        protected override List<Result> Execute(IQuery query) {
            var shortcut = query.Arguments[0];
            var linkPath = query.Arguments[1];
            var description = query.Arguments.Length > 2 ? string.Join(" ", query.Arguments.Skip(2)) : "";
            return new List<Result> {
                CreateResult(shortcut, linkPath, description)
            };
            ;
        }

        private Result CreateResult(string shortCut, string linkPath, string description) {
            Debug.WriteLine(shortCut + " => " + description);

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
    }
}