using System;
using System.Linq;

namespace Wox.Plugin.Links {
    public class Link {
        public string Shortcut { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public LinkType Type { get; set; }

        public ShortCutMatch Matches(string input) {
            if (string.IsNullOrWhiteSpace(input)) {
                return ShortCutMatch.Fail;
            }

            var startsWithHash = input.First() == '#';
            if (input.Length == 1 && startsWithHash)
                return new ShortCutMatch(0, this);
            if (startsWithHash) {
                input = input.Substring(1);
            }

            //Doesn't make sense to use, because Wox resort results and doesn't care about order of results in Plugin output
            var lastIndex = -1;
            var searchIndex = 0;
            foreach (var part in input) {
                var index = Shortcut.IndexOf(part.ToString(), lastIndex + 1,
                    StringComparison.InvariantCultureIgnoreCase);
                if (index <= lastIndex) {
                    return ShortCutMatch.Fail;
                }

                searchIndex += index - lastIndex - 1;

                lastIndex = index;
            }

            return Shortcut.ContainsCaseInsensitive(input) 
                ? new ShortCutMatch(searchIndex, this) 
                : ShortCutMatch.Fail;
        }

        public class ShortCutMatch {
            public int Index { get; }

            public Link Link { get; }

            public bool Matches { get; }

            public ShortCutMatch(int index, Link link) {
                Index = index;
                Link = link;
                Matches = index >= 0;
            }

            public static readonly ShortCutMatch Fail = new ShortCutMatch(-1, null);
        }
    }
}