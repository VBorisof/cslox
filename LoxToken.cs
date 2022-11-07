namespace CsLox
{
    public class LoxToken
    {
        public LoxTokenType TokenType { get; }
        public string Lexeme { get; }
        public object? Literal { get; }

        public int Line { get; }
        public int Column { get; }
        public int Length { get; }


        public LoxToken (
            LoxTokenType tokenType,
            string lexeme,
            object? literal,
            int line,
            int column,
            int length
        )
        {
            TokenType = tokenType;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
            Column = column;
            Length = length;
        }

        public override string ToString()
        {
            return $"{TokenType} {Lexeme} {Literal}";
        }
    }
}

