using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking.SharedKernel.Models
{
    public class CheckIn
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        [ForeignKey("Place")]
        public int PlaceId { get; set; }

        public virtual User User { get; set; }

        public virtual Place Place { get; set; }
    }
}
