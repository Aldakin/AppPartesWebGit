using AppPartes.IntegrationTests.Seedwork.Fixtures;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppPartes.IntegrationTests.Spec.Logic
{
    [Collection(nameof(Collection.TestServer))]
    public class WorkPartInformationTests
    {
        private readonly ServerFixture _serverFixture;
        public WorkPartInformationTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }
        [Fact]
        public async Task SelectedCompanyReadOt_Return_XXX()
        {
            //Arrange
            var client = _serverFixture.GetTestClient();
            //Act
            var response = await client.GetAsync("/workpartinformation/SelectedCompanyReadOt?iEntidad=0,idAldakinUser=0");
            //var response= await client.PostAsync("/main/InsertLine?strEntidad=0,strOt=0,strPresupuesto=0,strNivel1=0,strNivel2=0,strNivel3=0,strNivel4=0,strNivel5=0,strNivel6=0,strNivel7=0,strCalendario=0,strHoraInicio=0,strMinutoInicio=0,strHoraFin=0,strMinutoFin=0,bHorasViaje=0,bGastos=0,strParte=0,strPernoctacion=0,strObservaciones=0,strPreslin=0,strGastos=0",null);
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
