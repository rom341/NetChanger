using Microsoft.Extensions.Configuration;
using NetChanger;
using NetChangerTests;

namespace NetworkManagerTests
{
    public class NetworkManagerTests : IDisposable
    {
        private readonly NetworkManager _networkManager;

        public NetworkManagerTests()
        {
            // creation temporary directory for configuration file
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string>("PingAddress", "8.8.8.8"),
                    new KeyValuePair<string, string>("CheckInterval", "1000")
                ])
                .Build();

            _networkManager = new NetworkManager(config["PingAddress"]);
        }

        [Fact]
        public void ConnectToNetwork_Success()
        {
            PowerShellHelper.CreateLoopbackAdapter();

            _networkManager.ChangeNetworkToOneThatWorks();
            Assert.True(_networkManager.IsConnectedToInternet());

            PowerShellHelper.RemoveLoopbackAdapter();
        }

        [Fact]
        public void ConnectToNetwork_Failure()
        {
            _networkManager.ChangeNetworkToOneThatWorks();
            Assert.False(_networkManager.IsConnectedToInternet());
        }

        public void Dispose()
        {
            PowerShellHelper.RemoveLoopbackAdapter();
        }
    }
}