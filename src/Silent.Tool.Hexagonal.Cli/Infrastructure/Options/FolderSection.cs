namespace Silent.Tool.Hexagonal.Cli.Infrastructure.Options
{
    public class FolderSection
    {
        public const string Name = "folders";

        public string ClientsFolder { get; set; }

        public string DocsFolder { get; set; }

        public string SamplesFolder { get; set; }

        public string ServicesFolder { get; set; }

        public string WebAppsFolder { get; set; }
    }
}
