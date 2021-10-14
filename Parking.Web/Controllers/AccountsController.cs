using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Parking.Core.Enums;
using Parking.Core.Managers;
using Parking.SharedKernel.Models;
using Parking.Web.ViewModels.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking.Web.Controllers
{
    [Authorize(Roles = Roles.OfficeManager)]
    public class AccountsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserTokenManager _userTokenManager;
        private readonly CheckInManager _checkInManager;
        private readonly PlaceManager _placeManager;
        private readonly IPasswordValidator<User> _passwordValidator;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountsController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            UserTokenManager userTokenManager, 
            CheckInManager checkInManager, 
            PlaceManager placeManager,
            IPasswordValidator<User> passwordValidator,
            IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userTokenManager = userTokenManager;
            _checkInManager = checkInManager;
            _placeManager = placeManager;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.ContactNumber,
                };

                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Employee);
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        public async Task<ActionResult> Delete(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            _checkInManager.Delete(user.CheckIns);
            _placeManager.DeleteUserPlaces(user.UserPlaces);

            if (user != null)
                await _userManager.DeleteAsync(user);

            return RedirectToAction("List", "Users");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword()
        {
            User user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {

                    IdentityResult result = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);

                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(model.NewPassword), "Пользователь не найден");
                }
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            InitViewDataRoleName();

            return View(new LoginViewModel());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            InitViewDataRoleName();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(model.Password), "Неправильный логин и (или) пароль");
                return View(model);
            }

            var isChecked = await CheckRoleAsync(model.Role, nameof(model.Role));

            if (!isChecked)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            Response.Cookies.Delete(CookieTypes.LoginRole);
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> CreateNewToken()
        {
            var user = await _userManager.GetUserAsync(User);
            var token = _userTokenManager.CreateNewToken(user.Id);

            return View((object)token);
        }

        public async Task<bool> CheckRoleAsync(string roleName, string fieldName)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return false;

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                ModelState.AddModelError(fieldName, "У вас нет такой роли");
                return false;
            }

            Response.Cookies.Append(CookieTypes.LoginRole, roleName);
            return true;

        }

        public void InitViewDataRoleName()
        {
            var roles = new Dictionary<string, string>
            {
                { Roles.Employee, "Сотрудник" },
                { Roles.OfficeManager, "Офис менеджер" }
            };

            ViewData["RoleName"] = new SelectList(roles, "Key", "Value");
        }
    }
}
