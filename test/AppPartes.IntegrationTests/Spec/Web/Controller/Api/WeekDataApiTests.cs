using AppPartes.IntegrationTests.Seedwork.Fixtures;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppPartes.IntegrationTests.Spec.Web.Controller.Api
{
    [Collection(nameof(Collection.TestServer))]
    public class WeekDataApiTests
    {
        private readonly ServerFixture _serverFixture;

        public WeekDataApiTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }
        [Fact]
        public async Task SelectPayer_Set0_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/SelectPayer?cantidad=0,cantidad2=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectPayer_SetX_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/SelectPayer?cantidad=1,cantidad2=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectPayer_Set0_X_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/SelectPayer?cantidad=0,cantidad2=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task SelectPayer_SetX_X_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/SelectPayer?cantidad=1,cantidad2=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task DeleteLineFunction_Set0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/DeleteLineFunction?cantidad=0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task DeleteLineFunction_SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/DeleteLineFunction?cantidad=1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CloseFunction_Set0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/CloseFunction?strDataSelected=");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task CloseFunction_SetX_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/weekdataapi/CloseFunction?strDataSelected=2020-01-01");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
