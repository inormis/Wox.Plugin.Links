using System;
using System.Linq;

namespace Wox.Plugin.Links {
    public class QueryInstance : IQuery {
        public QueryInstance(Query query)
            : this(query.RawQuery) {
        }

        public QueryInstance(string rawQuery) {
            if (string.IsNullOrWhiteSpace(rawQuery)) {
                return;
            }

            var args = rawQuery.Split(' ');
            FirstSearch = args.FirstOrDefault() ?? "";
            SecondToEndSearch = rawQuery.Length >= FirstSearch.Length
                ? rawQuery.Substring(FirstSearch.Length).Trim()
                : "";
            Arguments = SecondToEndSearch.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }

        public string FirstSearch { get; } = "";
        public string[] Arguments { get; } = new string[0];
        public string SecondToEndSearch { get; } = "";
    }
}