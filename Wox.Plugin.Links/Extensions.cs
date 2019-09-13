using System;
using System.Linq;
using System.Text.RegularExpressions;

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

        public static bool NotEmpty(this string value) {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool MatchShortcut(this string value, string input) {
            if (string.IsNullOrWhiteSpace(input)) {
                return false;
            }

            var startsWithHash = input.First() == '#';
            if (input.Length == 1 && startsWithHash)
                return true;
            if (startsWithHash) {
                input = input.Substring(1);
            }

            if (value.ContainsCaseInsensitive(input)) {
                return true;
            }

            var inputs = input.SplitCamelCase();
            if (inputs.Length < 2) {
                return false;
            }

            var lastIndex = -1;
            foreach (var part in inputs) {
                var index = value.IndexOf(part, StringComparison.InvariantCultureIgnoreCase);
                if (index <= lastIndex) {
                    return false;
                }

                lastIndex = index;
            }

            return true;
        }

        private static string[] SplitCamelCase(this string source) {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }
    }
}