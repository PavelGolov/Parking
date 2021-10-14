using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parking.Core.Enums;
using Parking.Core.Managers;
using Parking.Infrastructure.Data;
using Parking.SharedKernel.Models;
using Parking.Web.Helpers;
using System.Linq;

namespace Parking.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlaceManager _placeManager;
        public HomeController(PlaceManager placeManager)
        {
            _placeManager = placeManager;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var loginRole = Request.Cookies[CookieTypes.LoginRole];

                if (loginRole == Roles.OfficeManager)
                    return RedirectToAction("OfficeManager");
                else
                    return RedirectToAction("Employee");
            }
            else
                return RedirectToAction("Login","Account");
        }
        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult OfficeManager()
        {
            var places = _placeManager.GetPlaces().ToList();
            var mapViewModels = PlaceHelper.GetMap(places);

            return View(mapViewModels);
        }
        public IActionResult Employee()
        {
            var places = _placeManager.GetPlaces().ToList();
            var mapViewModels = PlaceHelper.GetMap(places);

            return View(mapViewModels);
        }
    }
}
