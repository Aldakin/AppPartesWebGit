using AppPartes.IntegrationTests.Seedwork.Fixtures;
using AppPartes.Logic;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace AppPartes.IntegrationTests.Spec.Web.Controller.Api
{
    [Collection(nameof(Collection.TestServer))]
    public class MainDataApiTests
    {
        private readonly ServerFixture _serverFixture;
        public MainDataApiTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }
        [Fact]
        public async Task SelectedEntityOt_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectedEntityOt/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectedEntityClient_ShouldReturnHttp204()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectedEntityClient/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task ClientSelected_ShouldReturnHttp204()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/ClientSelected/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task OtSelected_ShouldReturnHttp204()
        {            
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/OtSelected/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task GetLevel_ShouldReturnHttp204()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task GetLevel1_ShouldReturnHttp204()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel1/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task GetLevel2_ShouldReturnHttp204()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel2/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task WeekSummary_ShouldReturnHttp204()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/WeekSummary?cantidad=2020-01-01");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task SelectPayer_ShouldReturnHttp204()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectPayer?cantidad=0,cantidad2=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
