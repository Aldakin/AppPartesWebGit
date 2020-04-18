using System.Net.Http;
using System.Threading.Tasks;
using AppPartes.Logic;
using AppPartes.Web;
using AppPartes.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AppPartes.IntegrationTests.Seedwork.Fixtures
{
    public class ServerFixture
    {
        private readonly TestServer _server;

        public ServerFixture()
        {
            var builder = new WebHostBuilder()
                .UseStartup<TestStartup>()
                .ConfigureServices(services =>
                {
                    var dataLogicMock = Mock.Of<ILoadIndexController>();
                    Mock.Get(dataLogicMock).Setup(dl => dl.LoadMainController(It.IsAny<int>()))
                        .ReturnsAsync(new MainDataViewLogic());
                    services.AddScoped<ILoadIndexController>(provider => dataLogicMock);
                })
                .ConfigureAppConfiguration((context, b) =>
                {
                    context.HostingEnvironment.ApplicationName = typeof(HomeController).Assembly.GetName().Name;
                });

            _server = new TestServer(builder);
        }

        public HttpClient GetTestClient()
        {
            return _server.CreateClient();
        }
    }
}
