using System;
using System.Collections.Generic;

namespace Poster.Template
{
    public class Tokenizer
    {
        private readonly string _template;
        private List<Token> _tokens;
        private int _position;

        public List<Token> Tokens
        {
            get
            {
                if (_tokens == null)
                {
                    _tokens = Tokenize();
                }

                return _tokens;
            }
        }

        public Tokenizer(string template)
        {
            _template = template;
        }

        private List<Token> Tokenize()
        {
            // Final output
            List<Token> tokens = new List<Token>();

            // LIFO stack to keep track of nested collection tokens, e.g. if, each
            Stack<Token> open = new Stack<Token>();

            // We're working on this token
            Token current = null;

            // Keep track of line and column numbers
            int lineCount = 1;
            int lastLinePosition = 0;

            for (_position = 0; _position < _template.Length; _position++)
            {
                // If we've reached a new line, mark that position so we can calculate the token's position in the overall text stream
                if (Peek("\n"))
                {
                    lineCount++;
                    lastLinePosition = _position;
                }

                // Look for the beginning of a new non-Text token
                //  - Skip check if we're already working on a non-Text token
                //  - Yes, that means we don't catch dumb syntax errors (e.g. "@{@{}") until the template is composited later
                if ((current == null || current.Type == TokenType.Text) && Peek("@{"))
                {
                    // We don't have an end tag for Text tokens, so we end any current Text token when we detect the beginning of a non-Text token
                    if (current != null)
                    {
                        if (open.Count > 0)
                        {
                            // Add to current open container if we have one
                            open.Peek().Tokens.Add(current);
                        }
                        else
                        {
                            // Otherwise, add this one to the ended list
                            tokens.Add(current);
                        }
                    }

                    // Store current position here
                    // Next() advances the stream, and we want to know the current starting position of this token, not where Next() ends up
                    int position = _position;

                    if (Next("@{if "))
                    {
                        current = new Token {
                            Type = TokenType.If,
                            Line = lineCount,
                            Column = position - lastLinePosition,
                            Position = position
                        };
                    }
                    else if (Next("@{each "))
                    {
                        current = new Token { Type = 
                            TokenType.Each,
                            Line = lineCount,
                            Column = position - lastLinePosition,
                            Position = position
                        };
                    }
                    else if (Next("@{reference "))
                    {
                        current = new Token {
                            Type = TokenType.Reference,
                            Line = lineCount,
                            Column = position - lastLinePosition,
                            Position = position
                        };
                    }
                    else if (Next("@{import "))
                    {
                        current = new Token {
                            Type = TokenType.Import,
                            Line = lineCount,
                            Column = position - lastLinePosition,
                            Position = position
                        };
                    }
                    else if (Next("@{"))
                    {
                        current = new Token {
                            Type = TokenType.Output,
                            Line = lineCount,
                            Column = position - lastLinePosition,
                            Position = position
                        };
                    }
                }
                // Look for the end of a non-Text token
                //  - Skip check for token end if we're working on a Text token
                else if (current != null && current.Type != TokenType.Text && Next("}"))
                {
                    // Check current token for 'close this' marker
                    //  - If we're closing a collection token (each, if), we need to do some further checking
                    if (current.Value[0] == '/')
                    {
                        TokenType closed = (TokenType) Enum.Parse(typeof(TokenType), current.Value.ToString().TrimStart('/'), true);

                        // Expect closed to be the same as last open
                        if (open.Count == 0 || open.Peek().Type != closed)
                        {
                            throw new Exception($"Unmatched end tag '@{{/{closed.ToString().ToLower()}}}' at {current.Position}");
                        }

                        Token token = open.Pop();

                        if (open.Count > 0)
                        {
                            open.Peek().Tokens.Add(token);
                        }
                        else
                        {
                            tokens.Add(token);
                        }
                    }
                    else
                    {
                        // The 'open this' token for a collection
                        //  - Add this to our stack so we can keep track of nesting
                        if (current.IsContainer())
                        {
                            open.Push(current);
                        }
                        // A non-collection token
                        else
                        {
                            // If we have a currently open collection token, add this to its collection
                            if (open.Count > 0)
                            {
                                open.Peek().Tokens.Add(current);
                            }
                            // ..or add this directly to 'ended' list
                            else
                            {
                                tokens.Add(current);
                            }
                        }
                    }

                    // TODO: Don't create whitespace-only tokens between script tokens?

                    current = null;
                }
                // Assume this character will be appended to the current token's Value
                //  - Create a new Text token if necessary
                else
                {
                    if (current == null)
                    {
                        current = new Token
                        {
                            Type = TokenType.Text,
                            Line = lineCount,
                            Column = _position - lastLinePosition,
                            Position = _position
                        };
                    }

                    current.Value.Append(_template[_position]);
                }
            }

            // Sanity check
            //  - Has our template closed all collection tokens?
            if (open.Count > 0)
            {
                throw new Exception($"Unmatched start tag '@{{{open.Peek().Type.ToString().ToLower()} {open.Peek().Value}}}' at {open.Peek().Position}");
            }
            //  - Has our last non-Text token been closed properly?
            if (current != null && current.Type != TokenType.Text)
            {
                throw new Exception($"Incomplete tag '@{{{(current.IsContainer() ? current.Type.ToString().ToLower() + " " : string.Empty)}{current.Value}' at {current.Position}");
            }

            // Do we need to close our last trailing Text token?
            //  - This is not an error condition, because we don't explicitly mark the end of Text tokens
            if (current != null)
            {
                tokens.Add(current);
            }

            return tokens;
        }

        private bool Peek(string find)
        {
            if (_position + find.Length >= _template.Length)
            {
                return false;
            }

            for (int i = 0; i < find.Length; i++)
            {
                if (_template[_position + i] != find[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool Next(string find)
        {
            if (Peek(find))
            {
                // Yes, our string occurs next
                // Set the pointer to the end of the occurence
                _position = _position + find.Length - 1;

                return true;
            }

            return false;
        }
    }
}
