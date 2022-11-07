using System.Collections.Generic;
using System.Linq;

namespace CsLox
{
    public class LoxScanner
    {
        private readonly ConsoleLogger _logger;

        private string _source = "";
        private string _currentLine = "";

        private int _start = 0;
        private int _lineNumber = 0;
        private int _totalLines;
        private int _columnIndex = 0;

        private bool _isInMultilineComment = false;

        public bool IsSuccess { get; private set; }

        public LoxScanner(ConsoleLogger logger)
        {
            _logger = logger;
        }


        public List<LoxToken> ScanTokens(string source)
        {
            IsSuccess = true;

            var tokens = new List<LoxToken>();
            _source = source;
            var lines = source.Split("\n");
            _totalLines = lines.Count();

            _logger.Debug($"Scanning {_totalLines} lines.");

            // Go line by line.
            foreach(var line in lines)
            {
                _lineNumber++;
                _columnIndex = 0;
                _currentLine = line;

                _logger.Debug($"Scan line {_lineNumber}: `{_currentLine}`.");

                // Scan each character in the line.
                while (_columnIndex < _currentLine.Length)
                {
                    var token = ScanToken(_currentLine[_columnIndex]);
                    if (token != null)
                    {
                        tokens.Add(token);
                    }
                }
            }

            tokens.Add(new LoxToken(LoxTokenType.EOF, "", null, _lineNumber, 0, 0));

            return tokens;
        }

        private LoxToken? ScanToken(char c)
        {
            _start = _columnIndex;
            ++_columnIndex;

            if (_isInMultilineComment)
            {
                if (c == '*' && IsMatch('/'))
                {
                    _isInMultilineComment = false;
                }
                return null;
            }
            switch(c)
            {
                case '(': return MakeToken(LoxTokenType.LeftParen);
                case ')': return MakeToken(LoxTokenType.RightParen);
                case '{': return MakeToken(LoxTokenType.LeftBrace);
                case '}': return MakeToken(LoxTokenType.RightBrace);
                case ',': return MakeToken(LoxTokenType.Comma);
                case '.': return MakeToken(LoxTokenType.Dot);
                case '-': return MakeToken(LoxTokenType.Minus);
                case '+': return MakeToken(LoxTokenType.Plus);
                case ';': return MakeToken(LoxTokenType.Semicolon);
                case '*': return MakeToken(LoxTokenType.Star);
                // Special handling for some multi-char tokens:
                case '!':
                    return MakeToken(IsMatch('=') 
                          ? LoxTokenType.BangEqual 
                          : LoxTokenType.Bang);
                case '=':
                    return MakeToken(IsMatch('=') 
                          ? LoxTokenType.DoubleEqual 
                          : LoxTokenType.Equal);
                case '<':
                    return MakeToken(IsMatch('=') 
                          ? LoxTokenType.LessEqual 
                          : LoxTokenType.Less);
                case '>':
                    return MakeToken(IsMatch('=') 
                          ? LoxTokenType.GreaterEqual 
                          : LoxTokenType.Greater);
                case '/':
                {
                    // We have a comment
                    if (IsMatch('/')) {
                        while (!IsLineEnd())
                        {
                            ++_columnIndex;
                        }
                    }
                    else if (IsMatch('*')) {
                        _isInMultilineComment = true;
                        while (!IsLineEnd())
                        {
                            ++_columnIndex;
                        }
                    }
                    else {
                        return MakeToken(LoxTokenType.Slash);
                    }

                    break;
                }

                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                case '"':
                {
                    var token = ReadString();
                    if (token != null)
                    {
                        return token;
                    }
                    break;
                }

                default:
                {
                    if (IsDigit(c))
                    {
                        var token = ReadNumber();
                        if (token != null)
                        {
                            return token;
                        }
                    }
                    else if (IsAlpha(c))
                    {
                        var token = ReadIdentifier();
                        if (token != null)
                        {
                            return token;
                        }
                    }
                    else
                    {
                        IsSuccess = false;

                        _logger.Error(
                            _lineNumber,
                            _columnIndex,
                            _currentLine,
                            $"Unexpected character: `{c}`"
                        );
                    }
                    return null;
                }
            }

            return null;
        }

        private LoxToken MakeToken(LoxTokenType type)
        {
            return MakeToken(type, null);
        }

        private LoxToken MakeToken(LoxTokenType type, object? literal)
        {
            string text = _currentLine.Substring(_start, _columnIndex-_start);
            return new LoxToken(
                tokenType: type,
                lexeme: text,
                literal: literal,
                _lineNumber,
                _columnIndex,
                _columnIndex-_start
            );
        }

        private LoxToken? ReadString()
        {
            while (Peek() != '"' && !IsLineEnd())
            {
                ++_columnIndex;
            }

            if (IsLineEnd())
            {
                _logger.Error(
                    _lineNumber,
                    _columnIndex+1,
                    _currentLine,
                    "Unterminated string."
                );
                return null;
            }

            ++_columnIndex;

            string text = _currentLine.Substring(_start, _columnIndex-_start);
            string val = _currentLine.Substring(_start+1, _columnIndex-_start-2);
            return new LoxToken(
                tokenType: LoxTokenType.String,
                lexeme: text,
                literal: val,
                _lineNumber,
                _columnIndex,
                _columnIndex-_start
            );
        }

        private LoxToken? ReadNumber()
        {
            while (IsDigit(Peek()))
            {
                ++_columnIndex;
            }

            if (Peek() == '.' && IsDigit(Peek(1)))
            {
                ++_columnIndex;

                while (IsDigit(Peek()))
                {
                    ++_columnIndex;
                }
            }

            string text = _currentLine.Substring(_start, _columnIndex-_start);
            double val = double.Parse(text);
            return new LoxToken(
                tokenType: LoxTokenType.Number,
                lexeme: text,
                literal: val,
                _lineNumber,
                _columnIndex,
                _columnIndex-_start
            );
        }
        
        private LoxToken? ReadIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                ++_columnIndex;
            }

            string text = _currentLine.Substring(_start, _columnIndex - _start);
            bool isKeyword = LoxKeywords.Keywords.TryGetValue(text, out LoxTokenType type);

            if (! isKeyword)
            {
                type = LoxTokenType.Identifier;
            }
            return MakeToken(type);
        }

        private bool IsMatch(char expected)
        {
            if (IsLineEnd())
            {
                return false;
            }
            if (_currentLine[_columnIndex] != expected)
            {
                return false;
            }

            ++_columnIndex;
            return true;
        }

        private char Peek(int num = 0)
        {
            if (IsLineEnd())
            {
                return '\0';
            }
            if (_columnIndex + num >= _currentLine.Length)
            {
                return '\0';
            }

            return _currentLine[_columnIndex+num];
        }

        private bool IsLineEnd()
        {
            return _columnIndex >= _currentLine.Length;
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z')
                   || (c >= 'A' && c <= 'Z')
                   || c == '_';
        }
        private bool IsAlphaNumeric(char c)
        {
            return IsDigit(c) || IsAlpha(c);
        }
    }
}

