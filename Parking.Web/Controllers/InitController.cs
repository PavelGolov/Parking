using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parking.Core.Enums;
using Parking.Core.Managers;
using Parking.SharedKernel.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Web.Controllers
{
    public class InitController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public InitController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        [AllowAnonymous]
        public async Task<IActionResult> Init()
        {
            if (!await _roleManager.RoleExistsAsync(Roles.Employee))
                await _roleManager.CreateAsync(new IdentityRole() { Name = Roles.Employee });

            if (!await _roleManager.RoleExistsAsync(Roles.OfficeManager))
                await _roleManager.CreateAsync(new IdentityRole() { Name = Roles.OfficeManager });

            var user = new User
            {
                LastName = "Test",
                FirstName = "Account",
                Email = "golov@amm.vsu.ru",
                UserName = "golov@amm.vsu.ru",
                PhoneNumber = "89300124913",
            };

            if (!_userManager.Users.Any(u => u.Email == user.Email))
            {
                await _userManager.CreateAsync(user, "Ghblevfqyjdsqgfhjkm1!");
                await _userManager.AddToRoleAsync(user, Roles.Employee);
                await _userManager.AddToRoleAsync(user, Roles.OfficeManager);
            }

            return RedirectToAction("Login", "Accounts");
        }
    }
}
