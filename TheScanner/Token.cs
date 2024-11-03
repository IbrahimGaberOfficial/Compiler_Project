namespace TheScanner
{
    public struct Token
    {
        public string Lexeme;
        public TokenType Type;
        public string ErrorMessage;

        public Token(string lexeme, TokenType type, string errorMessage = "")
        {
            Lexeme = lexeme;
            Type = type;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return Type == TokenType.Error
                ? $"Error: {ErrorMessage} at '{Lexeme}'"
                : $"Token: {Lexeme}, Type: {Type}";
        }
    }
}


