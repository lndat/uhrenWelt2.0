using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using uhrenWelt.ViewModels.User;
using uhrenWelt.Models;
using uhrenWelt.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace uhrenWelt.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly ICartService _cartService;

    public UserController(ILogger<UserController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, ICartService cartService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
        _cartService = cartService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        var user = new AppUser
        {
            Email = registerViewModel.Email,
            UserName = registerViewModel.Email,
            Title = registerViewModel.Title,
            FirstName = registerViewModel.FirstName,
            LastName = registerViewModel.LastName,
            Street = registerViewModel.Street,
            City = registerViewModel.City,
            Zip = registerViewModel.Zip
        };

        if (!ModelState.IsValid) return View();

        var result = await this._userManager.CreateAsync(user, registerViewModel.Password);

        if (result.Succeeded)
        {
            var confirmationToken = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "User", new { userId = user.Id, token = confirmationToken }, Request.Scheme);

            await _emailService
            .SendEmail("lnedza.dev@icloud.com", user.Email, "Please confirm your email", $"Click <a href=\"" + confirmationLink + "\">here</a> to confirm your email address");

            return RedirectToAction(nameof(SuccessRegistration));
        }
        else
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("Register", error.Description);
        }

        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return View();

        var appUser = await _userManager.FindByEmailAsync(loginViewModel.Email);
        var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, true);

        if (appUser == null)
        {
            ModelState.AddModelError("Login", "Login failed. User not found.");
            return View();
        }

        var emailStatus = await _userManager.IsEmailConfirmedAsync(appUser);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        else
        {
            if (result.IsLockedOut)
                ModelState.AddModelError("Login", "You are locked out.. please try again later.");
            else if (!emailStatus)
                ModelState.AddModelError("Login", "Login failed. Please confirm your email.");
            else if (!result.Succeeded)
                ModelState.AddModelError("Login", "Email or Password invalid");
            else
                ModelState.AddModelError("Login", "Login failed. Please contact the Administrator.");
        }

        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return View("ResetLinkConfirmation");
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // string scheme = Url.ActionContext.HttpContext.Request.Scheme;

            var callbackUrl = Url.Action(nameof(ResetPassword), "User", new { token, email = user.Email }, Request.Scheme);
            await _emailService.SendEmail("lnedza.dev@icloud.com", user.Email, "Reset your password", "To reset your password click <a href=\"" + callbackUrl + "\">here</a>");
            return RedirectToAction("ResetLinkConfirmation", "User");
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return View(resetPasswordViewModel);

        var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
        if (user == null)
            RedirectToAction(nameof(ResetPassword));

        var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
        if (!resetPassResult.Succeeded)
        {
            foreach (var error in resetPassResult.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return View();
        }
        return RedirectToAction(nameof(ResetPasswordConfirmation));
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetLinkConfirmation()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await this._userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var result = await this._userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _cartService.CreateCartAsync(user);
                await _signInManager.SignInAsync(user, true);
                return View("ConfirmEmail");
            }
        }

        return View("RegistrationError");
    }

    [HttpGet]
    public IActionResult SuccessRegistration()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RegistrationError()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
