namespace CsLox
{
    public enum LoxTokenType
    {
        LeftParen, RightParen,
        LeftBrace, RightBrace,
        Comma, Dot,
        Minus, Plus,
        Semicolon,
        Slash,
        Star,

        Bang, BangEqual,
        Equal, DoubleEqual,
        Greater, GreaterEqual,
        Less, LessEqual,

        Identifier, String, Number, Boolean,

        True, False,
        And, Or,
        If, Else,
        Class, Fun, Super, Return,
        While, For,
        Var, This,
        Nil,

        EOF,
    }
}

