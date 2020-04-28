using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers {
    public class AccountController : Controller {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController (UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  ILogger<AccountController> logger) 
        {
            this.logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register () {
            return View ();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register (RegisterViewModel model) {
            if (ModelState.IsValid) {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync (user, model.Password);
                if (result.Succeeded) {
                    var token = await userManager.GenerateChangeEmailTokenAsync (user, model.Password);

                    var confirmationLink = Url.Action ("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Warning, confirmationLink);

                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUser", "Administrator");
                    }

                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = "Before you login, please confirm your email, by clicking on the confirmation link we have mailed you";
                    return View("Error");
                }
                foreach (var error in result.Errors) {
                    ModelState.AddModelError ("", error.Description);
                }
            }
            return View (model);
        }

        [AcceptVerbs ("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse (string email) {
            var user = await userManager.FindByEmailAsync (email);
            if (user == null) {
                return Json (true);
            } else {
                return Json ($"Email {email} is already in use.");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login (string returnUrl) {
            LoginViewModel model = new LoginViewModel () {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync ()).ToList ()
            };
            return View (model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login (LoginViewModel model, string returnUrl) {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList ();
            if (ModelState.IsValid) {
                var user = await userManager.FindByEmailAsync (model.Email);

                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync (user, model.Password))) {
                    ModelState.AddModelError (string.Empty, "Email not confirmed. Please confirm your email before Login.");
                    return View (model);
                }

                var result = await signInManager.PasswordSignInAsync (model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded) {
                    if (!string.IsNullOrEmpty (returnUrl) && Url.IsLocalUrl (returnUrl)) {
                        return Redirect (returnUrl);
                    } else {
                        return RedirectToAction ("Index", "Home");
                    }
                }

                ModelState.AddModelError (string.Empty, "Invalid Login Attempt");
            }
            return View (model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout () {
            await signInManager.SignOutAsync ();
            return RedirectToAction ("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword () {
            return View ();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin (string provider, string returnUrl) {
            var redirectUrl = Url.Action ("ExternalLoginCallBack", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties (provider, redirectUrl);
            return new ChallengeResult (provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack (string returnUrl = null, string remoteError = null) {
            returnUrl = returnUrl ?? Url.Content ("~/");

            LoginViewModel loginViewModel = new LoginViewModel () {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync ()).ToList ()
            };

            if (remoteError != null) {
                ModelState.AddModelError (string.Empty, $"Error from external provider: {remoteError}");

                return View ("Login", loginViewModel);
            }

            var info = await signInManager.GetExternalLoginInfoAsync ();
            if (info == null) {
                ModelState.AddModelError (string.Empty, $"Error loading external login information.");

                return View ("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue (ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null) {
                user = await userManager.FindByEmailAsync (email);
                if (user != null && !user.EmailConfirmed) {
                    ModelState.AddModelError (string.Empty, "Email not confirmed. Please confirm your email before Login.");
                    return View ("Login", loginViewModel);
                }
            }

            var signinResult = await signInManager.ExternalLoginSignInAsync (info.LoginProvider, info.ProviderKey, isPersistent : false, bypassTwoFactor : true);

            if (signinResult.Succeeded) {
                return LocalRedirect (returnUrl);
            } else {
                if (email != null) {
                    user = await userManager.FindByEmailAsync (email);

                    if (user == null) {
                        user = new ApplicationUser () {
                        UserName = info.Principal.FindFirstValue (ClaimTypes.Email),
                        Email = info.Principal.FindFirstValue (ClaimTypes.Email)
                        };

                        await userManager.CreateAsync (user);
                    }

                    await userManager.AddLoginAsync (user, info);
                    await signInManager.SignInAsync (user, isPersistent : false);

                    return LocalRedirect (returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from : {info.LoginProvider}";
                ViewBag.ErrorMessage = $"Please contact support on mailmeanil97@gmail.com";
                return View ("Error");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("Index", "Employee");
            }

            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"The user Id {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if(result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email can't be confirmed";
            return View("Error");
        }


    }
}