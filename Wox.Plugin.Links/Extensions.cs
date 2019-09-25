using System;

namespace Wox.Plugin.Links {
    public static class Extensions {
        public static bool ContainsCaseInsensitive(this string value, string part) {
            if (string.IsNullOrWhiteSpace(part)) {
                return false;
            }

            return value.IndexOf(part, StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        public static bool StartsWithCaseInsensitive(this string value, string part) {
            if (string.IsNullOrWhiteSpace(part)) {
                return false;
            }

            return value.IndexOf(part, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}