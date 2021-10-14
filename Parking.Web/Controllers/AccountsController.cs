using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parking.Core.Enums;
using Parking.Core.Managers;
using Parking.SharedKernel.Models;
using Parking.Web.ViewModels.Accounts;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Web.Controllers
{
    [Authorize(Roles = Roles.OfficeManager)]
    public class AccountsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly CheckInManager _checkInManager;
        private readonly PlaceManager _placeManager;

        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, 
            RoleManager<IdentityRole> roleManager,CheckInManager checkInManager, PlaceManager placeManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _checkInManager = checkInManager;
            _placeManager = placeManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
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
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
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
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
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
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                var isChecked = await CheckRoleAsync(model.Email, model.Role);
                if (result.Succeeded && isChecked)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
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

        [AllowAnonymous]
        public async Task<IActionResult> Init()
        {
            await _roleManager.CreateAsync(new IdentityRole() { Name = Roles.Employee });
            await _roleManager.CreateAsync(new IdentityRole() { Name = Roles.OfficeManager });
            var user = new User
            {
                LastName = "Test",
                FirstName ="Account",
                Email = "golov@amm.vsu.ru",
                UserName = "golov@amm.vsu.ru",
                PhoneNumber = "89300124913",
            };
            await _userManager.CreateAsync(user, "Ghblevfqyjdsqgfhjkm1!");
            await _userManager.AddToRoleAsync(user, Roles.Employee);
            await _userManager.AddToRoleAsync(user, Roles.OfficeManager);

            return RedirectToAction("Login");
        }
        public async Task<bool> CheckRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return false;

            if (roleName == "Сотрудник")
            {
                Response.Cookies.Append(CookieTypes.LoginRole, Roles.Employee);
                return true;
            }

            var isOfficeManager = await _userManager.IsInRoleAsync(user, Roles.OfficeManager);
            if ((roleName == "Офис менеджер"))
            {
                if (isOfficeManager)
                {
                    Response.Cookies.Append(CookieTypes.LoginRole, Roles.OfficeManager);
                    return true;
                }

                if (!isOfficeManager)
                {
                    ModelState.AddModelError("", "У вас нет такой роли");
                    return false;
                }
            }
            return true;

        }
    }
}
