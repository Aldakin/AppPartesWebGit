using AppPartes.IntegrationTests.Seedwork.Fixtures;
using AppPartes.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text;
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

        //public class MockHttpContextBase : HttpContextBase
        //{
        //    public override IPrincipal User { get; set; }

        //}

        //[Fact]
        //public async Task InsertLine_ShouldReturnHttp200()
        //{
        //    //Arrange
        //    var client = _serverFixture.GetTestClient();
        //    //Act
        //    var response = await client.GetAsync("/main/InsertLine?strEntidad=0,strOt=0,strPresupuesto=0,strNivel1=0,strNivel2=0,strNivel3=0,strNivel4=0,strNivel5=0,strNivel6=0,strNivel7=0,strCalendario=0,strHoraInicio=0,strMinutoInicio=0,strHoraFin=0,strMinutoFin=0,bHorasViaje=0,bGastos=0,strParte=0,strPernoctacion=0,strObservaciones=0,strPreslin=0,strGastos=0");
        //    //var response= await client.PostAsync("/main/InsertLine?strEntidad=0,strOt=0,strPresupuesto=0,strNivel1=0,strNivel2=0,strNivel3=0,strNivel4=0,strNivel5=0,strNivel6=0,strNivel7=0,strCalendario=0,strHoraInicio=0,strMinutoInicio=0,strHoraFin=0,strMinutoFin=0,bHorasViaje=0,bGastos=0,strParte=0,strPernoctacion=0,strObservaciones=0,strPreslin=0,strGastos=0",null);
        //    //Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //}

        //strEntidad=0,strOt=0,strPresupuesto=0,strNivel1=0,strNivel2=0,strNivel3=0,strNivel4=0,strNivel5=0,strNivel6=0,strNivel7=0,strCalendario=0,strHoraInicio=0,strMinutoInicio=0,strHoraFin=0,strMinutoFin=0,bHorasViaje=0,bGastos=0,strParte=0,strPernoctacion=0,strObservaciones=0,strPreslin=0,strGastos=0
    }
}
