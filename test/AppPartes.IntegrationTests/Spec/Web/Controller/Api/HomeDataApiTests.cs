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
    public class HomeDataApiTests
    {
        private readonly ServerFixture _serverFixture;
        public HomeDataApiTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }
        [Fact]
        public async Task UpdateEntityData_Send_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/homedataapi/UpdateEntityData/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task UpdateEntityData_Send_1_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/homedataapi/UpdateEntityData/1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GenerateCsvData_Send_0_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/homedataapi/GenerateCsvData/0");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GenerateCsvData_Send_1_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/homedataapi/GenerateCsvData/1");
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
