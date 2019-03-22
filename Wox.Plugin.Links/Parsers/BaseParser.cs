using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Wox.Plugin.Links.Parsers {
    public abstract class BaseParser : IParser {
        protected const string PluginKey = "link";

        public virtual ParserPriority Priority { get; } = ParserPriority.Normal;

        private readonly Regex CommandMatch;

        protected BaseParser(string commandKey) {
            CommandMatch = new Regex($@"^--{commandKey}\b|^{commandKey}\b|^-{commandKey[0]}\b",
                RegexOptions.IgnoreCase);
        }

        public virtual bool TryParse(IQuery query, out List<Result> results) {
            results = new List<Result>();

            if (CommandMatch.IsMatch(query.FirstSearch) && CustomParse(query)) {
                results = Execute(query);
                return true;
            }

            return false;
        }

        protected virtual bool CustomParse(IQuery query) {
            return true;
        }

        protected List<Result> GetErrorResult(string message, string subTitle = null) {
            return new List<Result> {
                new Result {
                    Title = message,
                    SubTitle = subTitle,
                    IcoPath = @"icon.png",
                    Action = context => false
                }
            };
        }

        protected abstract List<Result> Execute(IQuery query);
    }
}