using AppPartes.Data.Models;
using AppPartes.Logic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;

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
            var dataLogicMock = Mock.Of<IDataLogic>();
            services.AddScoped<IDataLogic>(provider => dataLogicMock);
        }
    }
}
