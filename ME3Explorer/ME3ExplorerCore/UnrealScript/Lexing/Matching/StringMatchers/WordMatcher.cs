﻿using System.Collections.Generic;
using System.Linq;
using ME3ExplorerCore.UnrealScript.Compiling.Errors;
using ME3ExplorerCore.UnrealScript.Lexing.Tokenizing;
using ME3ExplorerCore.UnrealScript.Utilities;

namespace ME3ExplorerCore.UnrealScript.Lexing.Matching.StringMatchers
{
    public class WordMatcher : TokenMatcherBase<string>
    {
        private readonly List<KeywordMatcher> Delimiters;

        public WordMatcher(List<KeywordMatcher> delimiters)
        {
            Delimiters = delimiters ?? new List<KeywordMatcher>();
        }

        protected override Token<string> Match(TokenizableDataStream<string> data, ref SourcePosition streamPos, MessageLog log)
        {
            SourcePosition start = new SourcePosition(streamPos);
            string peek = data.CurrentItem;
            string word = null;
            loopStart:
            while (!data.AtEnd() && !string.IsNullOrWhiteSpace(peek) 
                && (Delimiters.All(d => d.Keyword != peek))
                && peek != "\"" && peek != "'")
            {
                word += peek;
                data.Advance();
                peek = data.CurrentItem;
            }

            //HACK: there are variable names that include the c++ scope operator '::' for some godforsaken reason
            if (peek == ":" && data.LookAhead(1) == ":")
            {
                word += peek;
                data.Advance();
                peek = data.CurrentItem;
                word += peek;
                data.Advance();
                peek = data.CurrentItem;
                goto loopStart;
            }

            if (word != null)
            {
                streamPos = streamPos.GetModifiedPosition(0, data.CurrentIndex - start.CharIndex, data.CurrentIndex - start.CharIndex);
                SourcePosition end = new SourcePosition(streamPos);
                return new Token<string>(TokenType.Word, word, start, end);
            }
            return null;
        }
    }
}
