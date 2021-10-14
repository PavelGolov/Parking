using System.ComponentModel.DataAnnotations.Schema;

namespace Parking.SharedKernel.Models
{
    public class UserPlace
    {
        public int Id { get; set; }
        [ForeignKey("Place")]
        public int PlaceId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual Place Place { get; set; }
    }
}
