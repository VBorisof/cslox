using System.Collections.Generic;

namespace CsLox
{
    public static class LoxKeywords
    {
        public static readonly Dictionary<string, LoxTokenType> Keywords;

        static LoxKeywords()
        {
            Keywords = new Dictionary<string, LoxTokenType>
            {
                { "and",    LoxTokenType.And },
                { "class",  LoxTokenType.Class },
                { "else",   LoxTokenType.Else },
                { "false",  LoxTokenType.False },
                { "for",    LoxTokenType.For },
                { "fun",    LoxTokenType.Fun },
                { "if",     LoxTokenType.If },
                { "nil",    LoxTokenType.Nil },
                { "or",     LoxTokenType.Or },
                { "return", LoxTokenType.Return },
                { "super",  LoxTokenType.Super },
                { "this",   LoxTokenType.This },
                { "true",   LoxTokenType.True },
                { "var",    LoxTokenType.Var },
                { "while",  LoxTokenType.While },
            };
        }
    }
}

