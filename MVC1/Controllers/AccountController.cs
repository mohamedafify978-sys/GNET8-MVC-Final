using GYMsystem.DAL.Models;
using GYMSystem.BLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MVC1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(user, model.password, model.RememberMe, false);
            if (result.Succeeded)
            {
                logger.LogInformation($"User:{user.UserName} is signed  in.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            else if (result.IsLockedOut)
            {
                logger.LogWarning($"User:{user.UserName} failed to sign in.");
                ModelState.AddModelError("InvalidLogin", "This account is locked ,Try Again later");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model);
            }

        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {

            return View();
        } 
    }
}
