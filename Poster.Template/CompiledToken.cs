using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Poster.Template
{
    public class CompiledToken<TContext> where TContext : IContext
    {
        public TokenType Type { get; set; }

        public string Value { get; set; }

        public Script Script { get; set; }

        public List<CompiledToken<TContext>> Tokens { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public int Position { get; set; }

        public CompiledToken()
        {
            Tokens = new List<CompiledToken<TContext>>();
        }

        public static List<CompiledToken<TContext>> Compile(List<Token> source, ScriptOptions scriptOptions = null)
        {
            List<CompiledToken<TContext>> tokens = new List<CompiledToken<TContext>>();

            scriptOptions = scriptOptions ?? ScriptOptions.Default;

            foreach (Token token in source)
            {
                if (token.Type == TokenType.Reference)
                {
                    scriptOptions = scriptOptions.AddReferences(token.Value.ToString());
                }
                else if (token.Type == TokenType.Import)
                {
                    scriptOptions = scriptOptions.AddImports(token.Value.ToString());
                }
                else
                {
                    tokens.Add(Compile(token, scriptOptions));
                }
            }

            return tokens;
        }

        public static CompiledToken<TContext> Compile(Token source, ScriptOptions scriptOptions = null)
        {
            scriptOptions = scriptOptions ?? ScriptOptions.Default;

            CompiledToken<TContext> token = new CompiledToken<TContext>
            {
                Type = source.Type,
                Value = source.Value.ToString(),
                Line = source.Line,
                Column = source.Column,
                Position = source.Position
            };

            if (source.Type == TokenType.Each)
            {
                token.Script = CSharpScript.Create<IEnumerable>(token.Value, scriptOptions, typeof(TContext));
                token.Script.Compile();
            }
            else if (source.Type == TokenType.If)
            {
                token.Script = CSharpScript.Create<bool>(token.Value, scriptOptions, typeof(TContext));
                token.Script.Compile();
            }
            else if (source.Type == TokenType.Output)
            {
                token.Script = CSharpScript.Create<object>(token.Value, scriptOptions, typeof(TContext));
                token.Script.Compile();
            }

            if (source.IsContainer())
            {
                token.Tokens = Compile(source.Tokens, scriptOptions);
            }

            return token;
        }

        public static async Task<List<object>> EvaluateAsync(List<CompiledToken<TContext>> tokens, TContext context)
        {
            List<object> evaluations = new List<object>();

            foreach (CompiledToken<TContext> token in tokens)
            {
                evaluations.AddRange(await EvaluateAsync(token, context));
            }

            return evaluations;
        }

        public static async Task<List<object>> EvaluateAsync(CompiledToken<TContext> token, TContext context)
        {
            List<object> evaluations = new List<object>();

            if (token.Type == TokenType.Text)
            {
                evaluations.Add(token.Value);
            }

            else if (token.Type == TokenType.Output)
            {
                evaluations.Add((await token.Script.RunAsync(context)).ReturnValue);
            }

            else if (token.Type == TokenType.If && (bool)(await token.Script.RunAsync(context)).ReturnValue)
            {
                foreach (CompiledToken<TContext> ifToken in token.Tokens)
                {
                    foreach (var result in await EvaluateAsync(ifToken, context))
                    {
                        evaluations.Add(result);
                    }
                }
            }

            else if (token.Type == TokenType.Each)
            {
                foreach (var current in (IEnumerable)(await token.Script.RunAsync(context)).ReturnValue)
                {
                    context.Current = current;

                    foreach (CompiledToken<TContext> eachToken in token.Tokens)
                    {
                        foreach (var result in await EvaluateAsync(eachToken, context))
                        {
                            evaluations.Add(result);
                        }
                    }
                }

                context.Current = null;
            }

            return evaluations;
        }
    }
}
