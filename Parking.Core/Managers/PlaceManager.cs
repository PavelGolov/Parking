using Parking.Infrastructure.Data;
using Parking.SharedKernel.Models;
using System.Collections.Generic;
using System.Linq;

namespace Parking.Core.Managers
{
    public class PlaceManager : BaseManager
    {
        public PlaceManager(ParkingContext dataContext) : base(dataContext)
        {
        }
        public Place GetPlace(int placeId)
        {
            return DataContext.Places.FirstOrDefault(e => e.Id == placeId);
        }
        public IEnumerable<Place> GetPlaces()
        {
            return DataContext.Places;
        }
        public void Create(Place place)
        {
            DataContext.Add(place);
            DataContext.SaveChanges();
        }
        public void Delete(int placeId)
        {
            var place = DataContext.Places.Find(placeId);
            DataContext.Places.Remove(place);
            DataContext.SaveChanges();
        }
        public IEnumerable<Place> GetFreePlaces()
        {
            var freePlaces = DataContext.Places
                .Where(p => p.UserPlaces.Count() == 0 && p.CheckIns.Count() == 0);

            return freePlaces;
        }
        public void AddOwner(int placeId, string userId)
        {
            var userPlace = new UserPlace() 
            {
                PlaceId = placeId,
                UserId = userId
            };

            DataContext.Add(userPlace);
            DataContext.SaveChanges();
        }
        public void RemoveOwner(int placeId, string userId)
        {
            var userPlace = DataContext.UserPlaces.First(e => e.PlaceId == placeId && e.UserId == userId);
            DataContext.UserPlaces.Remove(userPlace);
            DataContext.SaveChanges();
        }
        public bool ExistPlace(int column, int row)
        {
            return DataContext.Places.Any(p => p.Column == column && p.Row == row);
        }
        public void DeleteUserPlaces(IEnumerable<UserPlace> userPlaces)
        {
            foreach (var userPlace in userPlaces)
                DataContext.Remove(userPlace);

            DataContext.SaveChanges();
        }
    }
}
