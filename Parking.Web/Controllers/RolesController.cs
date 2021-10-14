using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parking.Core.Enums;
using Parking.Infrastructure.Data;
using Parking.SharedKernel.Models;
using System.Threading.Tasks;

namespace Parking.Web.Controllers
{
    [Authorize(Roles = Roles.OfficeManager)]
    public class RolesController : Controller
    {
        UserManager<User> _userManager;
        public RolesController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> AddOfficeManager(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRoleAsync(user, Roles.OfficeManager);
            return RedirectToAction("List", "Users");
        }
        public async Task<IActionResult> RemoveOfficeManager(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.RemoveFromRoleAsync(user, Roles.OfficeManager);
            return RedirectToAction("List", "Users");
        }
    }
}
