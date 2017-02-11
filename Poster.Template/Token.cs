using System.Collections.Generic;
using System.Text;

namespace Poster.Template
{
    public class Token
    {
        public TokenType Type { get; set; }

        public StringBuilder Value { get; set; }

        public List<Token> Tokens { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public int Position { get; set; }

        public Token()
        {
            Value = new StringBuilder();
            Tokens = new List<Token>();
        }

        public bool IsContainer()
        {
            if (Type == TokenType.Each || Type == TokenType.If)
            {
                return true;
            }

            return false;
        }

        public static List<Token> Tokenize(string template)
        {
            return new Tokenizer(template).Tokens;
        }
    }
}
