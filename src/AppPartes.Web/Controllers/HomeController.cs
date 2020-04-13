using AppPartes.Data.Models;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppPartes.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //[AutoValidateAntiforgeryToken]
        public IActionResult Index(string strMessage = "")
        {
            ViewBag.Message = strMessage;
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login()
        {

            using (var context = new AldakinDbContext())
            {
                var user = context.Usuarios.Where(x => x.Name.Equals("460b244aa3e22b31a53018fc506f517f") && x.CodEnt == x.CodEntO).ToList();

                if (user.Count == 1)
                {
                    //var claims = new List<Claim>();
                    //claims.Add(new Claim(ClaimTypes.NameIdentifier, user.First().Idusuario.ToString()));
                    //claims.Add(new Claim(ClaimTypes.Name, user.First().Nombrecompleto.ToString()));
                    //claims.Add(new Claim("id", "69"));

                    var identity = new ClaimsIdentity();
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.First().Nombrecompleto.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.First().Idusuario.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Role, user.First().CodEntO.ToString()));
                    ///

                    //ClaimsPrincipal principal = new ClaimsPrincipal();

                    //var identity = (ClaimsIdentity)principal.Identity;
                    //    identity.AddClaim(new Claim(ClaimTypes.Name, user.First().Nombrecompleto.ToString()));
                    //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.First().Idusuario.ToString()));
                    //    identity.AddClaim(new Claim("id", "69"));

                    //await UserManager.AddClaimAsync(userId, new Claim("SomeClaimType", claimValue));
                    //ClaimsIdentity identity = new ClaimsIdentity();
                    //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.First().Idusuario.ToString()));
                    //identity.AddClaim(new Claim(ClaimTypes.Name, "hola"));// user.First().Nombrecompleto.ToString()
                    ////identity.AddClaim(new Claim("userid", "ejemplo"));//ejemplo de claim personalizado

                    //return View("Index");

                    //para borrar los claims 
                    //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Index", "Main");
                }
                else
                {
                    return RedirectToAction("Index", new { strMessage = "Consulte con administracion se han detectado vaiors usuarios con los mismos datos;" });
                }
            }

            //return View("Index"); 
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
