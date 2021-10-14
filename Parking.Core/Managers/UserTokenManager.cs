using Parking.Infrastructure.Data;
using Parking.SharedKernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parking.Core.Managers
{
    public class UserTokenManager : BaseManager
    {
        public UserTokenManager(ParkingContext dataContext) : base(dataContext)
        {
        }

        public string CreateNewToken(string userId)
        {
            DisableActiveToken(userId);

            var guid = Guid.NewGuid();

            var token = new UserToken
            {
                ApiKeyHash = guid.GetHashCode(),
                ExpirationDate = DateTime.Now.AddYears(1),
                UserId = userId,
            };

            DataContext.Add(token);
            DataContext.SaveChanges();

            return guid.ToString();
        }

        public UserToken GetToken(Guid token)
        {
            return DataContext.UserTokens.FirstOrDefault(t => t.ApiKeyHash == token.GetHashCode() && t.ExpirationDate > DateTime.Now);
        }

        private void DisableActiveToken(string userId)
        {
            var activeToken = DataContext.UserTokens.FirstOrDefault(t => t.UserId == userId && t.ExpirationDate > DateTime.Now);

            if (activeToken == null)
                return;

            activeToken.ExpirationDate = DateTime.Now;
            DataContext.Update(activeToken);
            DataContext.SaveChanges();
        }
    }
}
