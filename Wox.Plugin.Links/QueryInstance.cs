using System;

namespace Wox.Plugin.Links {
    public class QueryInstance : IQuery {
        public QueryInstance(Query query)
            : this(query.FirstSearch, query.SecondToEndSearch) {
        }

        public QueryInstance(string firstSearch, string secondToEndSearch) {
            FirstSearch = firstSearch;
            SecondToEndSearch = secondToEndSearch;
            Arguments = secondToEndSearch.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }

        public string FirstSearch { get; }
        public string[] Arguments { get; }
        public string SecondToEndSearch { get; }
    }
}