using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parking.Core.Enums;
using Parking.Core.Managers;
using Parking.SharedKernel.Models;
using System.Threading.Tasks;

namespace Parking.Web.Controllers
{
    [Authorize(Roles = Roles.Employee)]
    public class CheckInsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly CheckInManager _checkInManager;
        public CheckInsController(UserManager<User> userManager,CheckInManager checkInManager)
        {
            _userManager = userManager;
            _checkInManager = checkInManager;
        }
        public async Task<IActionResult> CheckIn(int placeId)
        {
            var  user = await _userManager.GetUserAsync(User);

            _checkInManager.CheckIn(placeId, user.Id);

            return RedirectToAction("Employee", "Home");
        }
    }
}
