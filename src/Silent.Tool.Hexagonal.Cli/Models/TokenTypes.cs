using Silent.Tool.Hexagonal.Cli.Models;

namespace Silent.Tool.Hexagonal.Cli
{
    public static class TokenTypes
    {
        public static Token Company = new Token("COMPANY_NAME");
        public static Token Service = new Token("SERVICE_NAME");
        public static Token WebApp = new Token("WEBAPP_NAME");
        public static Token ProjectType = new Token("PROJECT_TYPE");

        public static Token[] All = new[]
        {
            Company,
            Service,
            WebApp,
            ProjectType
        };
    }
}
