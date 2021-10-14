using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Core.Enums;
using Parking.Core.Managers;
using Parking.SharedKernel.Models;
using Parking.Web.ViewModels.Places;
using Parking.Web.ViewModels.Users;
using System.Linq;
using System.Threading.Tasks;
using IndexViewModel = Parking.Web.ViewModels.Places.IndexViewModel;

namespace Parking.Web.Controllers
{
    public class PlacesController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly PlaceManager _placeManager;
        public PlacesController(UserManager<User> userManager, PlaceManager placeManager)
        {
            _userManager = userManager;
            _placeManager = placeManager;
        }

        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult Index()
        {
            var places = _placeManager.GetPlaces().ToList();

            var indexViewModel = places.Select(p => new IndexViewModel()
            {
                Place = new PlaceViewModel(p),
                Users = p.UserPlaces.Select(up => new PersonViewModel(up.User)).ToList()
            }).ToList();

            return View(indexViewModel);
        }

        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult Create(PlaceViewModel placeViewModel)
        {
            if (_placeManager.ExistPlace(placeViewModel.Column, placeViewModel.Row))
                ModelState.AddModelError("", "Такое место уже существует");

            if (ModelState.IsValid)
            {
                var place = new Place()
                {
                    Id = placeViewModel.Id,
                    Column = placeViewModel.Column,
                    Row = placeViewModel.Row
                };

                _placeManager.Create(place);
                return RedirectToAction(nameof(Index));
            }
            return View(placeViewModel);
        }

        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult AddOwner(int? placeId, string userId)
        {
            if (placeId == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(userId))
            {
                _placeManager.AddOwner(placeId.Value, userId);

                return RedirectToAction(nameof(Index));
            }

            var place = _placeManager.GetPlace(placeId.Value);

            var placeOwners = place.UserPlaces.Select(up => up.User);
            var users = _userManager.Users.ToList();
            users.RemoveAll(u => placeOwners.Contains(u));

            var indexViewModel = new IndexViewModel()
            {
                Place = new PlaceViewModel(place),
                Users = users.Select(u => new PersonViewModel(u)).ToList()
            };

            return View(indexViewModel);
        }

        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult DeleteOwner(int placeId, string userId)
        {
            _placeManager.RemoveOwner(placeId, userId);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult Delete(int? placeId)
        {
            if (placeId == null)
            {
                return NotFound();
            }

            _placeManager.Delete(placeId.Value);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.Employee)]
        public async Task<IActionResult> ListMyPlaces()
        {
            var user = await _userManager.GetUserAsync(User);
            var userPlaces = user.UserPlaces
                .Select(up => up.Place)
                .Where(p => p.CheckIns.Count == 0)
                .ToList();

            if (userPlaces.Count == 1)
            {
                return RedirectToAction("CheckIn", "CheckIns", new { placeId = userPlaces.First().Id });
            }

            var listViewModel = new ListViewModel()
            {
                Places = userPlaces.Select(p => new PlaceViewModel(p)).ToList()
            };

            return View("List", listViewModel);
        }

        [Authorize(Roles = Roles.Employee)]
        public async Task<IActionResult> ListFreePlaces()
        {
            var freePlaces = _placeManager.GetFreePlaces().ToList();

            if (freePlaces.Count == 1)
            {
                return RedirectToAction("CheckIn", "CheckIns", new { placeId = freePlaces.First().Id });
            }

            var user = await _userManager.GetUserAsync(User);

            var listViewModel = new ListViewModel()
            {
                Places = freePlaces.Select(p => new PlaceViewModel(p)).ToList()
            };

            return  View("List",listViewModel);
        }
    }
}
