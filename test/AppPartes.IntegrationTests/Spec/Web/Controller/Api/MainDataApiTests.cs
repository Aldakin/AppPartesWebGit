using AppPartes.IntegrationTests.Seedwork.Fixtures;
using AppPartes.Logic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
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
        public async Task SelectedEntityOt_Set0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectedEntityOt?cantidad=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectedEntityOt__SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectedEntityOt?cantidad=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectedEntityClient_SET0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectedEntityClient?cantidad=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectedEntityClient_SETX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectedEntityClient?cantidad=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task ClientSelected_Set0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/ClientSelected?cantidad=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task ClientSelected_SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/ClientSelected?cantidad=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task OtSelected_Set0_ShouldReturnHttp200()
        {            
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/OtSelected?cantidad=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task OtSelected_SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/OtSelected?cantidad=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel_Set0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel?cantidad=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel_SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel?cantidad=3");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel1_Set0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel1?cantidad=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel1_SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel1?cantidad=4");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel2_Set0_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel2?cantidad=0,cantidad2=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel2_SetX_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel2?cantidad=1,cantidad2=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel2_Set0_X_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel2?cantidad=0,cantidad2=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetLevel2_SetX_X_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/GetLevel2?cantidad=1,cantidad=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task WeekSummary_Set0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();

            //Act
            var response = await client.GetAsync("/maindataapi/WeekSummary?cantidad=");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task WeekSummary_SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();

            //Act
            var response = await client.GetAsync("/maindataapi/WeekSummary?cantidad=2020-01-01");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectPayer_Set0_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectPayer?cantidad=0,cantidad2=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectPayer_SetX_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectPayer?cantidad=1,cantidad2=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectPayer_Set0_X_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectPayer?cantidad=0,cantidad2=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectPayer_SetX_X_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/maindataapi/SelectPayer?cantidad=1,cantidad2=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

}
