using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(AppPartes.Web.Areas.Identity.IdentityHostingStartup))]
namespace AppPartes.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}