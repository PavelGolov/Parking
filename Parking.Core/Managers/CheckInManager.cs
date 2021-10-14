using Parking.Infrastructure.Data;
using Parking.SharedKernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parking.Core.Managers
{
    public class CheckInManager : BaseManager
    {
        public CheckInManager(ParkingContext dataContext) : base(dataContext)
        {
        }
        public void CheckIn(int placeId, string userId)
        {
            if (DataContext.CheckIns.Any(e => e.UserId == userId))
            {
                var checkIn = DataContext.CheckIns.First(e => e.UserId == userId);
                DataContext.CheckIns.Remove(checkIn);
            }

            DataContext.CheckIns.Add(new CheckIn() { UserId = userId, PlaceId = placeId, Date = DateTime.Now });
            
            DataContext.SaveChanges();
        }
        public void Delete(IEnumerable<CheckIn> checkIns)
        {
            foreach(var checkIn in checkIns)
                DataContext.Remove(checkIn);

            DataContext.SaveChanges();
        }

        public void ResetCheckIns()
        {
            DataContext.RemoveRange(DataContext.CheckIns);
            DataContext.SaveChanges();
        }
    }
}
