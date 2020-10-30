using Xunit;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests
{
    public class InitializeCommandTests
    {
        [Fact]
        public void Test1()
        {
            // --help
            // init "Shop" --framework "net5.0"
            // add service "Orders" --framework "net5.0"
            // add service --help
            // add webapp "Marketplace" --framework "net5.0"
            // add webapp --help
            // add aggregate "Order" --service "Orders"
        }
    }
}
