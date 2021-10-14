using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Core.Enums;
using Parking.Core.Factories;
using System.Linq;
using Parking.Web.ViewModels.Places;
using Parking.Core.Managers;

namespace Parking.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlaceManager _placeManager;
        private readonly PlaceMapFactory _placeMapFactory;
        public HomeController(PlaceManager placeManager, PlaceMapFactory placeMapFactory)
        {
            _placeManager = placeManager;
            _placeMapFactory = placeMapFactory;
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
            var placeMap = _placeMapFactory.CreatePlaceMap(places);
            var mapViewModels = new MapViewModel(placeMap);

            return View(mapViewModels);
        }
        public IActionResult Employee()
        {
            var places = _placeManager.GetPlaces().ToList();
            var placeMap = _placeMapFactory.CreateOccupiedPlaceMap(places);
            var mapViewModels = new MapViewModel(placeMap);

            return View(mapViewModels);
        }
    }
}
