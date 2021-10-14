using Parking.Web.ViewModels.Users;
using System.Collections.Generic;

namespace Parking.Web.ViewModels.Places
{
    public class IndexViewModel
    {
        public PlaceViewModel Place { get; set; }
        public List<PersonViewModel> Users { get; set; }
    }
}
