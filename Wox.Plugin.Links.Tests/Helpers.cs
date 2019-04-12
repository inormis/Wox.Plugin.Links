using Wox.Plugin.Links;

namespace Wox.Links.Tests {
    public static class Helpers {
        public static IQuery AsQuery(this string search) {
            return new QueryInstance(search);
        }
    }
}