using System.Collections.Generic;
using System.Linq;

namespace Wox.Plugin.Links {
    public class LinksPlugin : IPlugin {
        private IEngine _engine;

        public List<Result> Query(Query query) {
            var result = _engine.Execute(new QueryInstance(query));
            return result.ToList();
        }

        public void Init(PluginInitContext context) {
            Startup.Initialize(new PluginContext(context));
            _engine = Startup.Resolve<IEngine>();
        }
    }
}