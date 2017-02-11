using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Template
{
    public class CompiledTemplate<TContext> where TContext : IContext
    {
        public List<CompiledToken<TContext>> Tokens { get; set; }

        public async Task<string> EvaluateAsync(TContext context)
        {
            StringBuilder content = new StringBuilder();

            foreach (var result in await CompiledToken<TContext>.EvaluateAsync(Tokens, context))
            {
                content.Append(result);
            }

            return content.ToString();
        }

        public CompiledTemplate()
        {
            Tokens = new List<CompiledToken<TContext>>();
        }

        public static CompiledTemplate<TContext> Compile(string template)
        {
            return Compile(Token.Tokenize(template));
        }

        public static CompiledTemplate<TContext> Compile(List<Token> tokens)
        {
            return new CompiledTemplate<TContext>
            {
                Tokens = CompiledToken<TContext>.Compile(tokens)
            };
        }
    }
}
