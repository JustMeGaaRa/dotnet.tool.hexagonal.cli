namespace Silent.Tool.Hexagonal.Cli.Infrastructure.Options
{
    public class GeneralOptions
    {
        public const string Name = "general";

        public FrameworkOptions Framework { get; set; }

        public FolderOptions Folders { get; set; }
    }
}
