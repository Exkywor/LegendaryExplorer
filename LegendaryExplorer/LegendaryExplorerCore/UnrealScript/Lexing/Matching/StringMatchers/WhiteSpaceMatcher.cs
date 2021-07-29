﻿using LegendaryExplorerCore.UnrealScript.Compiling.Errors;
using LegendaryExplorerCore.UnrealScript.Lexing.Tokenizing;
using LegendaryExplorerCore.UnrealScript.Utilities;

namespace LegendaryExplorerCore.UnrealScript.Lexing.Matching.StringMatchers
{
    public class WhiteSpaceMatcher : TokenMatcherBase<string>
    {
        protected override Token<string> Match(TokenizableDataStream<string> data, ref SourcePosition streamPos, MessageLog log)
        {
            var start = new SourcePosition(streamPos);
            bool whiteSpace = false;
            int newlines = 0;
            int column = streamPos.Column;
            while (!data.AtEnd() && string.IsNullOrWhiteSpace(data.CurrentItem))
            {
                whiteSpace = true;
                if (data.CurrentItem == "\n")
                {
                    newlines++;
                    column = 0;
                }
                else
                    column++;
                data.Advance();
            }

            if (whiteSpace)
            {
                streamPos = new SourcePosition(start.Line + newlines, column, data.CurrentIndex);
                var end = new SourcePosition(streamPos);
                return new Token<string>(TokenType.WhiteSpace, null, start, end);
            }
            return null;
        }
    }
}
