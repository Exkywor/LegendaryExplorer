﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LegendaryExplorerCore.UnrealScript.Compiling.Errors;
using LegendaryExplorerCore.UnrealScript.Lexing.Tokenizing;
using LegendaryExplorerCore.UnrealScript.Utilities;

namespace LegendaryExplorerCore.UnrealScript.Lexing.Matching.StringMatchers
{
    public class KeywordMatcher : TokenMatcherBase<string>
    {
        public string Keyword { get; }
        private readonly TokenType Type;
        private readonly List<KeywordMatcher> Delimiters;
        private readonly bool SubString;

        public KeywordMatcher(string keyword, TokenType type, List<KeywordMatcher> delims, bool allowSubString = true)
        {
            Type = type;
            Keyword = keyword;
            Delimiters = delims ?? new List<KeywordMatcher>();
            SubString = allowSubString;
        }

        protected override Token<string> Match(TokenizableDataStream<string> data, ref SourcePosition streamPos, MessageLog log)
        {
            var start = new SourcePosition(streamPos);
            foreach (char c in Keyword)
            {
                if (!string.Equals(data.CurrentItem, c.ToString(CultureInfo.InvariantCulture), StringComparison.CurrentCultureIgnoreCase))
                    return null;
                data.Advance();
            }

            string peek = data.CurrentItem;
            bool hasDelimiter = string.IsNullOrWhiteSpace(peek) || Delimiters.Any(c => c.Keyword == peek);
            if (SubString || (!SubString && hasDelimiter))
            {
                streamPos = streamPos.GetModifiedPosition(0, data.CurrentIndex - start.CharIndex, data.CurrentIndex - start.CharIndex);
                var end = new SourcePosition(streamPos);
                return new Token<string>(Type, Keyword, start, end);
            }
            return null;
        }
    }
}
