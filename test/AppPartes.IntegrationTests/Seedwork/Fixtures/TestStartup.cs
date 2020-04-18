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
            var workPartMock = Mock.Of<IWorkPartInformation>();
            Mock.Get(workPartMock).Setup(x => x.SelectedCompanyReadOt(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<SelectData>());
            services.AddScoped<IWorkPartInformation>(provider => workPartMock);
            var writeMock = Mock.Of<IWriteDataBase>();
            services.AddScoped<IWriteDataBase>(provider => writeMock);
            var loadIndexMock = Mock.Of<ILoadIndexController>();
            services.AddScoped<ILoadIndexController>(provider => loadIndexMock);
        }
    }
}
