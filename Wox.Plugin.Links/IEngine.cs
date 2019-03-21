using System.Collections.Generic;

namespace Wox.Plugin.Links {
    public interface IEngine {
        IEnumerable<Result> Execute(IQuery query);
    }
}