namespace Silent.Tool.Hexagonal.Cli.Infrastructure.Options
{
    public class ProjectsSection
    {
        public const string Name = "projects";

        public ProjectInfoSection Service { get; set; }

        public ProjectInfoSection WebApp { get; set; }

        public ProjectInfoSection UnitTest { get; set; }

        public ProjectInfoSection Client { get; set; }
    }
}
