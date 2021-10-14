using Microsoft.AspNetCore.Mvc;
using Parking.Core.Enums;
using Parking.Core.Managers;
using Parking.Web.Attributes;
using System;

namespace Parking.Web.ControllersApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CheckInsController : ControllerBase
    {
        private readonly CheckInManager _checkInManager;

        public CheckInsController(CheckInManager checkInManager)
        {
            _checkInManager = checkInManager;
        }

        [ApiKeyAuthorize(Roles = Roles.OfficeManager)]
        [HttpPost]
        public IActionResult ResetCheckIns()
        {
            try
            {
                _checkInManager.ResetCheckIns();
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
