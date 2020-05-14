using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using AppPartes.Web.Models;
using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AppPartes.Web.Controllers.Api;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;

namespace AppPartes.Web
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddRazorRuntimeCompilation().AddApplicationPart(typeof(Startup).Assembly);
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
            Mock.Get(workPartMock).Setup(x => x.ReadLevel2(0,0, It.IsAny<int>()))
                .ReturnsAsync(default(List<SelectData>));
            Mock.Get(workPartMock).Setup(x => x.ReadLevel2(It.IsAny<int>(),0, It.IsAny<int>()))
                .ReturnsAsync(default(List<SelectData>));
            Mock.Get(workPartMock).Setup(x => x.ReadLevel2(0, It.IsAny<int>(),  It.IsAny<int>()))
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
            Mock.Get(writeMock).Setup(x => x.DeleteWorkerLineAsync(0, It.IsAny<int>()))
                .ReturnsAsync(default(List<SelectData>));
            Mock.Get(writeMock).Setup(x => x.DeleteWorkerLineAsync(It.IsAny<int>(), It.IsAny<int>()))
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




        }
    }
}
