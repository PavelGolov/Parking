using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking.SharedKernel.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<CheckIn> CheckIns { get; set; }
        public virtual ICollection<UserPlace> UserPlaces { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; }
    }
}
