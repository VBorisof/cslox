using System.Collections.Generic;

namespace CsLox
{
    public class LoxParser
    {
        private readonly ConsoleLogger _logger;
        private readonly List<LoxToken> _tokens;
        private int _current;

        public LoxParser(ConsoleLogger logger, List<LoxToken> tokens)
        {
            _logger = logger;
            _tokens = tokens;
        }


        //
        // expression -> equality
        private Expression Expression()
        {
            return Equality();
        }

        //
        // equality -> comparison ( ("!=" | "==") comparison)* ;
        private Expression Equality()
        {
            var expr = Comparison();
            while (IsMatch(LoxTokenType.BangEqual, LoxTokenType.DoubleEqual))
            {
                var op = Previous();
                var right = Comparison();

                expr = new BinaryExpression(expr, op, right);
            }

            return expr;
        }

        //
        // comparison -> term ( (">" | ">=" | "<" | "<=") term )* ;
        private Expression Comparison()
        {
            var expr = Term();
            while (IsMatch(
                LoxTokenType.Greater, LoxTokenType.GreaterEqual,
                LoxTokenType.Less, LoxTokenType.LessEqual)
            )
            {
                var op = Previous();
                var right = Term();
                expr = new BinaryExpression(expr, op, right);
            }

            return expr;
        }

        //
        // term -> factor ( ("-" | "+") factor )* ;
        private Expression Term()
        {
            var expr = Factor();
            while (IsMatch(LoxTokenType.Minus, LoxTokenType.Plus))
            {
                var op = Previous();
                var right = Factor();
                expr = new BinaryExpression(expr, op, right);
            }

            return expr;
        }

        //
        // factor -> unary ( ("/" | "*") unary )* ;
        private Expression Factor()
        {
            var expr = Unary();
            while (IsMatch(LoxTokenType.Slash, LoxTokenType.Star))
            {
                var op = Previous();
                var right = Unary();
                expr = new BinaryExpression(expr, op, right);
            }

            return expr;
        }

        //
        // unary -> ( "-" | "!" ) unary | primary ;
        private Expression Unary()
        {
            if (IsMatch(LoxTokenType.Minus, LoxTokenType.Bang))
            {
                var op = Previous();
                var right = Unary();
                return new UnaryExpression(op, right);
            }

            return Primary();
        }

        // 
        // primary -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" ;
        private Expression Primary()
        {
            if (IsMatch(LoxTokenType.True))
            {
                return new LiteralExpression(true);
            }
            if (IsMatch(LoxTokenType.False))
            {
                return new LiteralExpression(false);
            }
            if (IsMatch(LoxTokenType.Nil))
            {
                return new LiteralExpression(null);
            }

            if (IsMatch(LoxTokenType.Number, LoxTokenType.String))
            {
                return new LiteralExpression(Previous().Literal);
            }

            if (IsMatch(LoxTokenType.LeftParen))
            {
                var expr = Expression();
                if (Consume(LoxTokenType.RightParen) == null)
                {
                    _logger.ParserError(
                        _tokens[_current].Line,
                        _tokens[_current].Column,
                        "Expected `)` after expression."
                    );
                    // TODO: Do we want an exception here..?
                    throw new System.Exception("Expected `)` after expression.");
                }
                return new GroupingExpression(expr);
            }
            _logger.ParserError(
                _tokens[_current].Line,
                _tokens[_current].Column,
                "Expected an expression."
            );

            // TODO: Do we want an exception here?
            throw new System.Exception("Expected an expression.");
        }

        private bool IsMatch(params LoxTokenType[] types)
        {
            foreach (var type in types)
            {
                if (PeekIsType(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private LoxToken? Consume(LoxTokenType type)
        {
            if (PeekIsType(type))
            {
                return Advance();
            }
            return null;
        }


        private bool PeekIsType(LoxTokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().TokenType == type;
        }
        private bool IsAtEnd()
        {
            return Peek().TokenType == LoxTokenType.EOF;
        }
        private LoxToken Peek()
        {
            return _tokens[_current];
        }
        private LoxToken Previous()
        {
            return _tokens[_current-1];
        }

        private LoxToken Advance()
        {
            if (!IsAtEnd())
            {
                ++_current;
                return Previous();
            }
            return Peek();
        }
    }
}

