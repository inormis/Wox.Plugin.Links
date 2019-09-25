using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.Links.Parsers;

namespace Wox.Plugin.Links {
    public interface IEngine {
        IEnumerable<Result> Execute(IQuery query);
    }

    public class Engine : IEngine {
        private readonly IEnumerable<IParser> _parsers;

        public Engine(IEnumerable<IParser> parsers) {
            _parsers = parsers.OrderByDescending(x => x.Priority).ToArray();
        }

        public IEnumerable<Result> Execute(IQuery query) {
            foreach (var parser in _parsers) {
                if (!parser.TryParse(query, out var results)) {
                    continue;
                }

                foreach (var result in results) {
                    yield return result;
                }

                yield break;
            }
        }
    }
}