using System;
using System.Linq;
using Wox.Plugin.Links;

namespace Wox.Links.Tests {
    public static class Helpers {
        public static IQuery AsQuery(this string search) {
            var split = search.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            return new QueryInstance(split.FirstOrDefault() ?? "", search.Substring(split[0].Length).Trim());
        }
    }
}