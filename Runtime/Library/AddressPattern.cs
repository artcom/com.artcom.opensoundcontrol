using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Artcom.OpenSoundControl.Library {
    /// <summary>
    ///     OSC describes a glob-like pattern matching to listen for multiple paths,
    ///     this class is able to transform it into a regex, which can be matched
    ///     again.
    /// </summary>
    [Serializable]
    public class AddressPattern {
        private const char SingleWildcard = '?';
        private const char MultipleWildcard = '*';
        private const char CharacterSetOpen = '[';
        private const char CharacterSetClose = ']';
        private const char CharacterSetNegation = '!';
        private const char StringSetOpen = '{';
        private const char StringSetClose = '}';
        private const char StringSetSeparator = ',';
        private readonly Regex compiledPattern;
        private readonly bool matchRegex;
        private readonly string originalPattern;

        public AddressPattern(string pattern) {
            originalPattern = pattern;
            var characterSet = false;
            var regexBuilder = new StringBuilder();
            foreach(var c in pattern) {
                switch(c) {
                    case SingleWildcard:
                        regexBuilder.Append('.');
                        matchRegex = true;
                        break;
                    case MultipleWildcard:
                        regexBuilder.Append(".*");
                        matchRegex = true;
                        break;
                    case CharacterSetOpen:
                        regexBuilder.Append('[');
                        characterSet = true;
                        matchRegex = true;
                        break;
                    case CharacterSetClose:
                        regexBuilder.Append(']');
                        characterSet = false;
                        break;
                    case CharacterSetNegation:
                        regexBuilder.Append(characterSet ? '^' : c);
                        break;
                    case StringSetOpen:
                        regexBuilder.Append('(');
                        matchRegex = true;
                        break;
                    case StringSetSeparator:
                        regexBuilder.Append('|');
                        break;
                    case StringSetClose:
                        regexBuilder.Append(')');
                        break;
                    default:
                        regexBuilder.Append(c);
                        break;
                }
            }
#if NET_2_0_SUBSET // .NET 2.0 Subset does not contain / support the Compiled Flag.
            compiledPattern = new Regex(regexBuilder.ToString());
#else
            compiledPattern = new Regex(regexBuilder.ToString(), RegexOptions.Compiled);
#endif
            }

        public bool IsMatch(string part) {
            return matchRegex ? compiledPattern.IsMatch(part) : part == originalPattern;
        }
    }
}