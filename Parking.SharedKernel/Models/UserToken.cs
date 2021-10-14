using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking.SharedKernel.Models
{
    public class UserToken
    {
        public int Id { get; set; }

        public int ApiKeyHash { get; set; }

        public DateTime ExpirationDate { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
