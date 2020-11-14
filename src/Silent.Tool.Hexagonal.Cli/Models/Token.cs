using System;

namespace Silent.Tool.Hexagonal.Cli.Models
{
    public class Token
    {
        public Token(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            Name = $"{{{name.ToUpper()}}}";
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}
