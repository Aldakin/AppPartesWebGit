using System.Net.Http;
using System.Threading.Tasks;
using AppPartes.Logic;
using AppPartes.Web;
using AppPartes.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using AppPartes.Web.Controllers.Api;
using System.Security.Claims;
using System;

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
                    Mock.Get(dataLogicMock).Setup(dl => dl.LoadMainControllerAsync(It.IsAny<int>()))
                        .ReturnsAsync(new MainDataViewLogic());
                    services.AddScoped<ILoadIndexController>(provider => dataLogicMock);

                    //IWorkPartInformation
                    var workPartMock = Mock.Of<IWorkPartInformation>();
                    Mock.Get(workPartMock).Setup(x => x.SelectedCompanyReadOt(0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.SelectedCompanyReadOt(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.SelectedCompanyReadClient(0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.SelectedCompanyReadClient(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.SelectedClient(0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.SelectedClient(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.SelectedOt(0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.SelectedOt(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.ReadLevelGeneral(0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.ReadLevelGeneral(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.ReadLevel1(0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.ReadLevel1(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.ReadLevel2(0, 0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.ReadLevel2(It.IsAny<int>(), 0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.ReadLevel2(0, It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.ReadLevel2(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.WeekHourResume(It.IsAny<DateTime>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(workPartMock).Setup(x => x.SelectedPayerAsync(0, 0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.SelectedPayerAsync(It.IsAny<int>(), 0, It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.SelectedPayerAsync(0, It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(workPartMock).Setup(x => x.SelectedPayerAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(new List<SelectData>());
                    services.AddScoped<IWorkPartInformation>(provider => workPartMock);
                    //IWriteDataBase
                    var writeMock = Mock.Of<IWriteDataBase>();
                    Mock.Get(writeMock).Setup(x => x.UpdateEntityDataOrCsvAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                        .ReturnsAsync("Hola mundo :)");//It.IsAny<string>()
                    Mock.Get(writeMock).Setup(x => x.DeleteWorkerLineAsync(0, It.IsAny<int>(),0))
                        .ReturnsAsync(default(List<SelectData>));
                    Mock.Get(writeMock).Setup(x => x.DeleteWorkerLineAsync(It.IsAny<int>(), It.IsAny<int>(),0))
                        .ReturnsAsync(new List<SelectData>());
                    Mock.Get(writeMock).Setup(x => x.CloseWorkerWeekAsync(It.IsAny<string>(), It.IsAny<int>()))
                        .ReturnsAsync(new SelectData());
                    Mock.Get(writeMock).Setup(x => x.CloseWorkerWeekAsync(null, It.IsAny<int>()))
                        .ReturnsAsync(new SelectData());
                    services.AddScoped<IWriteDataBase>(provider => writeMock);
                    //ILoadIndexController
                    var loadIndexMock = Mock.Of<ILoadIndexController>();
                    services.AddScoped<ILoadIndexController>(provider => loadIndexMock);
                    //IApplicationUserAldakin
                    var applicationUserAldakinMock = Mock.Of<IApplicationUserAldakin>();
                    Mock.Get(applicationUserAldakinMock).Setup(x => x.GetIdUserAldakin(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(100);
                    services.AddScoped<IApplicationUserAldakin>(provider => applicationUserAldakinMock);


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
