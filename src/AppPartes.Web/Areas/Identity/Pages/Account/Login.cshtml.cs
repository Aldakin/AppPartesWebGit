using AppPartes.Logic;
using AppPartes.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppPartes.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ILoadIndexController _ILoadIndexController;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager, ILoadIndexController iLoadIndexController)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _ILoadIndexController = iLoadIndexController;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }
        public LoginDataViewLogic lEntity { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Company { get; set; }
            [Required]
            //[EmailAddress]
            [DataType(DataType.Text)]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            lEntity = await _ILoadIndexController.LoadLoginControllerAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                try
                {
                    if (Convert.ToInt32(Input.Email) < 1000)
                    {
                        if (Convert.ToInt32(Input.Email) < 100)
                        {
                            if (Convert.ToInt32(Input.Email) < 10)
                            {
                                Input.Email = Input.Company + "000" + Input.Email;
                            }
                            else
                            {

                                Input.Email = Input.Company + "00" + Input.Email;
                            }
                        }
                        else
                        {
                            Input.Email = Input.Company + "0" + Input.Email;
                        }
                    }
                    else
                    {

                        Input.Email = Input.Company + Input.Email;
                    }
                }
                catch (Exception ex)
                {
                    lEntity = await _ILoadIndexController.LoadLoginControllerAsync();
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    lEntity = await _ILoadIndexController.LoadLoginControllerAsync();
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return Page();
                    ////Error Situation
                    //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    //return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
