﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.UnrealScript.Analysis.Visitors;
using ME3ExplorerCore.UnrealScript.Compiling.Errors;
using ME3ExplorerCore.UnrealScript.Language.Tree;
using ME3ExplorerCore.UnrealScript.Lexing.Tokenizing;
using ME3ExplorerCore.UnrealScript.Utilities;
using static ME3ExplorerCore.UnrealScript.Utilities.Keywords;

namespace ME3ExplorerCore.UnrealScript.Parsing
{
    public abstract class StringParserBase
    {
        protected MessageLog Log;
        protected TokenStream<string> Tokens;
        protected TokenType CurrentTokenType => Tokens.CurrentItem.Type;

        protected Token<string> CurrentToken => Tokens.CurrentItem;
        protected Token<string> PrevToken => Tokens.Prev();

        protected SourcePosition CurrentPosition => Tokens.CurrentItem.StartPos ?? new SourcePosition(-1, -1, -1);

        public static readonly List<ASTNodeType> SemiColonExceptions = new()
        {
            ASTNodeType.WhileLoop,
            ASTNodeType.ForLoop,
            ASTNodeType.ForEachLoop,
            ASTNodeType.IfStatement,
            ASTNodeType.SwitchStatement,
            ASTNodeType.CaseStatement,
            ASTNodeType.DefaultStatement,
            ASTNodeType.StateLabel,
        };

        public static readonly List<ASTNodeType> CompositeTypes = new()
        {
            ASTNodeType.Class,
            ASTNodeType.Struct,
            ASTNodeType.Enumeration,
            ASTNodeType.ObjectLiteral
        };

        protected ParseException ParseError(string msg, Token<string> token)
        {
            token.SyntaxType = EF.ERROR;
            return ParseError(msg, token.StartPos, token.EndPos);
        }

        protected ParseException ParseError(string msg, ASTNode node) => ParseError(msg, node.StartPos, node.EndPos);

        protected ParseException ParseError(string msg, SourcePosition start = null, SourcePosition end = null)
        {
            Log.LogError(msg, start, end);
            return new ParseException(msg);
        }

        protected void TypeError(string msg, Token<string> token)
        {
            token.SyntaxType = EF.ERROR;
            TypeError(msg, token.StartPos, token.EndPos);
        }

        protected void TypeError(string msg, ASTNode node) => TypeError(msg, node.StartPos, node.EndPos);

        protected void TypeError(string msg, SourcePosition start = null, SourcePosition end = null)
        {
            Log.LogError(msg, start, end);
        }

        public VariableIdentifier ParseVariableName()
        {
            VariableIdentifier var = TryParseVariable();
            if (var == null)
            {
                Log.LogError("Expected a variable name!", CurrentPosition);
                return null;
            }
            return var;
        }

        public VariableIdentifier TryParseVariable()
        {
            return (VariableIdentifier)Tokens.TryGetTree(VariableParser);
            ASTNode VariableParser()
            {
                var name = Consume(TokenType.Word);
                if (name == null) return null;

                if (Consume(TokenType.LeftSqrBracket) != null)
                {
                    var size = Consume(TokenType.IntegerNumber);
                    if (size == null)
                    {
                        throw ParseError("Expected an integer number for size!", CurrentPosition);
                    }

                    if (Consume(TokenType.RightSqrBracket) == null)
                    {
                        throw ParseError("Expected ']'!", CurrentPosition);
                    }

                    return new VariableIdentifier(name.Value, name.StartPos, name.EndPos, int.Parse(size.Value));
                }

                return new VariableIdentifier(name.Value, name.StartPos, name.EndPos);
            }
        }

        public VariableType TryParseType()
        {
            return (VariableType)Tokens.TryGetTree(TypeParser);
            ASTNode TypeParser()
            {
                if (Matches(ARRAY, EF.Keyword))
                {
                    var arrayToken = Tokens.Prev();
                    if (Consume(TokenType.LeftArrow) is null)
                    {
                        throw ParseError("Expected '<' after 'array'!", CurrentPosition);
                    }
                    var elementType = TryParseType();
                    if (elementType == null)
                    {
                        throw ParseError("Expected element type for array!", CurrentPosition);
                    }

                    if (elementType is DynamicArrayType)
                    {
                        throw ParseError("Arrays of Arrays are not supported!", elementType.StartPos, elementType.EndPos);
                    }
                    if (Consume(TokenType.RightArrow) is null)
                    {
                        throw ParseError("Expected '>' after array type!", CurrentPosition);
                    }
                    return new DynamicArrayType(elementType, arrayToken.StartPos, CurrentPosition);
                }

                if (Matches(DELEGATE, EF.Keyword))
                {
                    var delegateToken = Tokens.Prev();
                    if (Consume(TokenType.LeftArrow) is null)
                    {
                        throw ParseError("Expected '<' after 'delegate'!", CurrentPosition);
                    }

                    string functionName = "";
                    do
                    {
                        if (Consume(TokenType.Word) is Token<string> identifier)
                        {
                            identifier.SyntaxType = EF.Function;
                            if (functionName.Length > 0)
                            {
                                functionName += ".";
                            }
                            functionName += identifier.Value;
                        }
                        else
                        {
                            throw ParseError("Expected function name for delegate!", CurrentPosition);
                        }
                    } while (Matches(TokenType.Dot, EF.Function));
                    if (Consume(TokenType.RightArrow) is null)
                    {
                        throw ParseError("Expected '>' after function name!", CurrentPosition);
                    }
                    return new DelegateType(new Function(functionName, default, null, null, null), delegateToken.StartPos, CurrentPosition);
                }

                if (Consume(CLASS) is {} classToken)
                {
                    classToken.SyntaxType = EF.Keyword;
                    if (Consume(TokenType.LeftArrow) is null)
                    {
                        return new ClassType(new VariableType(OBJECT));
                    }

                    if (!(Consume(TokenType.Word) is {} classNameToken))
                    {
                        throw ParseError("Expected class name!", CurrentPosition);
                    }

                    classNameToken.SyntaxType = EF.TypeName;

                    if (Consume(TokenType.RightArrow) is null)
                    {
                        throw ParseError("Expected '>' after class name!", CurrentPosition);
                    }
                    return new ClassType(new VariableType(classNameToken.Value), classToken.StartPos, PrevToken.EndPos);
                }

                Token<string> type = Consume(TokenType.Word);
                if (type == null)
                {
                    return null;
                }

                type.SyntaxType = type.Value is INT or FLOAT or BOOL or BYTE or BIOMASK4 or STRING or STRINGREF or NAME ? EF.Keyword : EF.TypeName;
                return new VariableType(type.Value, type.StartPos, type.EndPos);
            }

        }

        #region Helpers

        public bool Matches(string str, EF syntaxType = EF.None)
        {
            bool matches = CurrentIs(str);
            if (matches)
            {
                if (syntaxType != EF.None)
                {
                    CurrentToken.SyntaxType = syntaxType;
                }
                Tokens.Advance();
            }
            return matches;
        }

        public bool Matches(params string[] strs)
        {
            bool matches = CurrentIs(strs);
            if (matches)
            {
                Tokens.Advance();
            }
            return matches;
        }

        public bool Matches(TokenType tokenType, EF syntaxType = EF.None)
        {
            if (Tokens.CurrentItem.Type == tokenType)
            {
                if (syntaxType != EF.None)
                {
                    CurrentToken.SyntaxType = syntaxType;
                }
                Tokens.Advance();
                return true;
            }

            return false;
        }

        public bool CurrentIs(string str)
        {
            return CurrentToken.Type != TokenType.EOF && CurrentToken.Value.Equals(str, StringComparison.OrdinalIgnoreCase);
        }
        public bool CurrentIs(TokenType tokenType)
        {
            return CurrentToken.Type == tokenType;
        }

        public bool CurrentIs(params string[] strs) => strs.Any(CurrentIs);

        public bool CurrentIs(params TokenType[] tokenTypes) => tokenTypes.Any(CurrentIs);

        public Token<string> Consume(TokenType tokenType) => Tokens.ConsumeToken(tokenType);

        public Token<string> Consume(params TokenType[] tokenTypes) => tokenTypes.Select(Consume).NonNull().FirstOrDefault();

        public Token<string> Consume(string str)
        {

            Token<string> token = null;
            if (CurrentIs(str))
            {
                token = CurrentToken;
                Tokens.Advance();
            }
            return token;
        }

        public Token<string> Consume(params string[] strs) => strs.Select(Consume).NonNull().FirstOrDefault();

        #endregion

        public Expression ParseLiteral()
        {
            Token<string> token = CurrentToken;
            if (Matches(TokenType.IntegerNumber))
            {
                int val = int.Parse(token.Value, CultureInfo.InvariantCulture);
                return new IntegerLiteral(val, token.StartPos, token.EndPos) { NumType = val >= 0 && val <= byte.MaxValue ? BYTE : INT };
            }

            if (Matches(TokenType.FloatingNumber))
            {
                return new FloatLiteral(float.Parse(token.Value, CultureInfo.InvariantCulture), token.StartPos, token.EndPos);
            }

            if (Matches(TokenType.StringLiteral))
            {
                return new StringLiteral(token.Value, token.StartPos, token.EndPos);
            }

            if (Matches(TokenType.NameLiteral))
            {
                return new NameLiteral(token.Value, token.StartPos, token.EndPos);
            }

            if (Matches(TokenType.StringRefLiteral))
            {
                return new StringRefLiteral(int.Parse(token.Value, CultureInfo.InvariantCulture), token.StartPos, token.EndPos);
            }

            if (Matches(TRUE, FALSE))
            {
                token.SyntaxType = EF.Keyword;
                return new BooleanLiteral(bool.Parse(token.Value), token.StartPos, token.EndPos);
            }

            if (Matches(NONE, EF.Keyword))
            {
                return new NoneLiteral(token.StartPos, token.EndPos);
            }

            if (CurrentIs(VECT) && Tokens.LookAhead(1).Type == TokenType.LeftParenth)
            {
                token.SyntaxType = EF.Keyword;
                Tokens.Advance();
                return ParseVectorLiteral();
            }

            if (CurrentIs(ROT) && Tokens.LookAhead(1).Type == TokenType.LeftParenth)
            {
                token.SyntaxType = EF.Keyword;
                Tokens.Advance();
                return ParseRotatorLiteral();
            }

            return null;
        }

        private Expression ParseRotatorLiteral()
        {
            var start = CurrentPosition;
            if (!Matches(TokenType.LeftParenth))
            {
                throw ParseError($"Expected '(' after '{ROT}' in rotator literal!", CurrentPosition);
            }

            int pitch = ParseInt();

            if (!Matches(TokenType.Comma))
            {
                throw ParseError("Expected ',' after pitch component in rotator literal!", CurrentPosition);
            }

            int yaw = ParseInt();

            if (!Matches(TokenType.Comma))
            {
                throw ParseError("Expected ',' after yaw component in rotator literal!", CurrentPosition);
            }

            int roll = ParseInt();

            if (!Matches(TokenType.RightParenth))
            {
                throw ParseError("Expected ')' after roll component in rotator literal!", CurrentPosition);
            }

            return new RotatorLiteral(pitch, yaw, roll, start, Tokens.Prev().EndPos);
        }

        private Expression ParseVectorLiteral()
        {
            var start = CurrentPosition;
            if (!Matches(TokenType.LeftParenth))
            {
                throw ParseError($"Expected '(' after '{VECT}' in vector literal!", CurrentPosition);
            }

            float x = ParseFloat();

            if (!Matches(TokenType.Comma))
            {
                throw ParseError("Expected ',' after x component in vector literal!", CurrentPosition);
            }

            float y = ParseFloat();

            if (!Matches(TokenType.Comma))
            {
                throw ParseError("Expected ',' after y component in vector literal!", CurrentPosition);
            }

            float z = ParseFloat();

            if (!Matches(TokenType.RightParenth))
            {
                throw ParseError("Expected ')' after z component in vector literal!", CurrentPosition);
            }

            return new VectorLiteral(x, y, z, start, Tokens.Prev().EndPos);
        }

        float ParseFloat()
        {
            bool isNegative = Matches(TokenType.MinusSign);
            if (!Matches(TokenType.FloatingNumber) && !Matches(TokenType.IntegerNumber))
            {
                throw ParseError("Expected number literal!", CurrentPosition);
            }

            var val = float.Parse(Tokens.Prev().Value, CultureInfo.InvariantCulture);
            return isNegative ? -val : val;
        }

        int ParseInt()
        {
            bool isNegative = Matches(TokenType.MinusSign);
            if (!Matches(TokenType.IntegerNumber))
            {
                throw ParseError("Expected integer literal!", CurrentPosition);
            }

            var val = int.Parse(Tokens.Prev().Value, CultureInfo.InvariantCulture);
            return isNegative ? -val : val;
        }
    }

    public class ParseException : Exception
    {
        public ParseException(string msg) : base(msg){}
    }
}
