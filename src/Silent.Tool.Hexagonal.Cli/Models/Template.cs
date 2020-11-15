using System;
using System.Linq;

namespace Silent.Tool.Hexagonal.Cli.Models
{
    public class Template
    {
        private Template(string format, string value, Token[] allTokens, Token[] unresolvedTokens)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));

            Format = format;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            AllTokens = allTokens ?? throw new ArgumentNullException(nameof(allTokens));
            UnresolvedTokens = unresolvedTokens ?? throw new ArgumentNullException(nameof(unresolvedTokens));
            IsResolved = !unresolvedTokens.Any();
        }

        public static Template FromFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));

            var tokens = TokenTypes.All
                .Where(x => format.Contains(x.Name))
                .ToArray();

            return new Template(format, format, tokens, tokens);
        }

        public string Format { get; }

        public string Value { get; }

        public Token[] AllTokens { get; }

        public Token[] UnresolvedTokens { get; }

        public bool IsResolved { get; }

        public override string ToString() => Value;

        public Template ResolveToken(Token token, string value)
        {
            var valueIsValid = !string.IsNullOrWhiteSpace(value);
            var updatedValue = valueIsValid ? Value.Replace(token.Name, value) : Value;
            var unresolvedTokens = TokenTypes.All
                .Where(x => updatedValue.Contains(x.Name))
                .ToArray();
            return new Template(Format, updatedValue, AllTokens, unresolvedTokens);
        }
    }
}
