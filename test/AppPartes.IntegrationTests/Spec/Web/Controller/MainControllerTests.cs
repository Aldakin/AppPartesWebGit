using AppPartes.IntegrationTests.Seedwork.Fixtures;
using AppPartes.Logic;
using AppPartes.Web.Controllers;
using AppPartes.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
//https://docs.microsoft.com/es-es/aspnet/core/mvc/controllers/testing?view=aspnetcore-3.1
namespace AppPartes.IntegrationTests.Spec.Web.Controller
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
        public async Task test_kk()
        {

        }

    }
}
