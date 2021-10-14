using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking.SharedKernel.Models
{
    public class Place
    {
        public int Id { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public virtual ICollection<CheckIn> CheckIns { get; set; }
        public virtual ICollection<UserPlace> UserPlaces { get; set; }
    }
}
