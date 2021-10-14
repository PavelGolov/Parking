using Parking.Infrastructure.Data;

namespace Parking.Core.Managers
{
    public abstract class BaseManager
    {
        protected readonly ParkingContext DataContext;
        protected BaseManager(ParkingContext dataContext)
        {
            DataContext = dataContext;
        }
    }
}
