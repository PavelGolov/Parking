using Parking.Core.Enums;

namespace Parking.Web.ViewModels.Places
{
    public class MapViewModel
    {
        public PlaceStates[,] Places { get; set; }
        public MapViewModel(PlaceStates[,] places)
        {
            Places = places;
        }

    }
}
