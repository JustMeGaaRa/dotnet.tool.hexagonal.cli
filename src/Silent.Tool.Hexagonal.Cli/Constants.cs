namespace Silent.Tool.Hexagonal.Cli
{
    public static class Constants
    {
        public static class Tokens
        {
            public const string CompanyTokenName = "{COMPANY_NAME}";
            public const string ServiceTokenName = "{SERVICE_NAME}";
            public const string WebAppTokenName = "{WEBAPP_NAME}";
            public const string ProjectTypeTokenName = "{PROJECT_TYPE}";
        }

        public static class OptionsTypes
        {
            public const string Service = "service";
            public const string WebApp = "webapp";
            public const string UnitTest = "unit_test";
            public const string Client = "client";
        }

        public static class ProjectTypes
        {
            public const string Api = "Api";
            public const string Domain = "Domain";
            public const string Infrastructure = "Infrastructure";
            public const string Web = "Web";
            public const string UnitTest = "Tests";
            public const string IntegrationTest = "IntegrationTests";
            public const string Client = "Client";
        }

        public const string ConfigFileName = ".hexaconfig";
    }
}
