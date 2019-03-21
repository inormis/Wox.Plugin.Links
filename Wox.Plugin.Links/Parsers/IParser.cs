using System.Collections.Generic;

namespace Wox.Plugin.Links.Parsers {
    public interface IParser {
        bool TryParse(IQuery query, out List<Result> results);
        ParserPriority Priority { get; }
    }
}