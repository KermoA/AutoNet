using AutoNet.Core.Domain;
using AutoNet.Models.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoNet.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ConfirmationEmail _confirmationEmail;

        public AccountsController
            (
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                ConfirmationEmail confirmationEmail
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _confirmationEmail = confirmationEmail;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel vm)

        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = vm.Email,
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    Email = vm.Email,
                };

                var result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { token, email = user.Email }, Request.Scheme);

                    bool emailSent = await _confirmationEmail.SendEmailAsync(user.Email, confirmationLink);

                    if (emailSent)
                    {
                        ViewBag.ErrorTitle = "Registration Successful";
                        ViewBag.ErrorMessage = "Please check your email to confirm your account.";
                        return View("RegistrationSuccess");
                    }

                    ViewBag.ErrorTitle = "Registration Successful";
                    ViewBag.ErrorMessage = "We could not send a confirmation email. Please contact support.";
                    return View("EmailError");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(vm);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            LoginViewModel vm = new()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError("", "Invalid Login Attempt");
            }

            return View(model);
        }

		[AllowAnonymous]
		[HttpPost]
		public IActionResult ExternalLogin(string provider, string returnUrl)
		{
			var redirectUrl = Url.Action(action: "ExternalLoginCallback", controller: "Accounts", values: new { ReturnUrl = returnUrl });

			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

			return new ChallengeResult(provider, properties);
		}

		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			LoginViewModel loginViewModel = new LoginViewModel
			{
				ReturnUrl = returnUrl,
				ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
			};
			if (remoteError != null)
			{
				ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
				return View("Login", loginViewModel);
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				ModelState.AddModelError(string.Empty, "Error loading external login information.");
				return View("Login", loginViewModel);
			}

			var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
				info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (signInResult.Succeeded)
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				var email = info.Principal.FindFirstValue(ClaimTypes.Email);
				if (email != null)
				{
					var user = await _userManager.FindByEmailAsync(email);
					if (user == null)
					{
						user = new ApplicationUser
						{
							UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
							Email = info.Principal.FindFirstValue(ClaimTypes.Email),
							FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
							LastName = info.Principal.FindFirstValue(ClaimTypes.GivenName)
						};
						await _userManager.CreateAsync(user);
					}
					await _userManager.AddLoginAsync(user, info);

					await _signInManager.SignInAsync(user, isPersistent: false);
					return LocalRedirect(returnUrl);
				}
				ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
				ViewBag.ErrorMessage = "Please contact support on info@dotnettutorials.net";
				return View("Error");
			}
		}

		[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return View("Error");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Accounts", new { email = model.Email, token = token }, Request.Scheme);

                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }
    }
}
