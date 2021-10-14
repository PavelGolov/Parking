using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Core.Enums;
using Parking.SharedKernel.Models;
using Parking.Web.ViewModels.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Web.Controllers
{
    public class UsersController : Controller
    {
        UserManager<User> _userManager;
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager; 
        }
        [Authorize(Roles = Roles.OfficeManager)]
        public async Task<IActionResult> List()
        {
            var users = await _userManager.Users.ToListAsync();
            var indexViewModel = users.Select(u => new IndexViewModel()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ContactNumber = u.PhoneNumber,
                Email = u.Email,
                IsOfficeManager = _userManager.IsInRoleAsync(u, Roles.OfficeManager).Result
            }).ToList();

            return View(indexViewModel);
        }
        [Authorize(Roles = Roles.OfficeManager)]
        public IActionResult Details(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return NotFound();
            }
            var user = _userManager.Users.FirstOrDefault(e => e.Id == userId);
            var detailsViewModel = new DetailsViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ContactNumber = user.PhoneNumber,
                Email = user.Email
            };

            return View(detailsViewModel);
        }

        [Authorize(Roles = Roles.OfficeManager)]
        public async Task<IActionResult> Edit(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);
            var person = new PersonViewModel(user);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.OfficeManager)]
        public async Task<IActionResult> Edit(PersonViewModel person)
        {

            if (ModelState.IsValid)
            {
                var user = await  _userManager.FindByIdAsync(person.Id);
                if (user== null)
                {
                    return NotFound();
                }

                user.FirstName = person.FirstName;
                user.LastName = person.LastName;
                user.PhoneNumber = person.ContactNumber;
                user.Email = person.Email;
                user.UserName = person.Email;

                await _userManager.UpdateAsync(user);

                return RedirectToAction("List");
            }
            return View(person);
        }
    }
}
