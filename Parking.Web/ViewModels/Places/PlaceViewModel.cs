using Parking.SharedKernel.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Parking.Web.ViewModels.Places
{
    public class PlaceViewModel
    {
        public PlaceViewModel()
        {
        }
        public PlaceViewModel(Place place)
        {
            Id = place.Id;
            Column = place.Column;
            Row = place.Row;
        }
        public int Id { get; set; }

        [Display(Name = "Номер строки")]
        public int Column { get; set; }

        [Display(Name = "Номер ряда")]
        public int Row { get; set; }
    }
}
