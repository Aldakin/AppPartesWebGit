﻿using AppPartes.IntegrationTests.Seedwork.Fixtures;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace AppPartes.IntegrationTests.Spec
{
    [Collection(nameof(Collection.TestServer))]
    public class MainControllerTests
    {
        private readonly ServerFixture _serverFixture;

        public MainControllerTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }

        [Fact]
        public async Task Index_ShouldReturnHttp200()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();

            //Act
            var response = await client.PostAsync("/main/entidadselectedot/1",null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}