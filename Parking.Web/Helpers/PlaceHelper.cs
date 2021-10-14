using Parking.Core.Enums;
using Parking.SharedKernel.Models;
using Parking.Web.ViewModels.Places;
using System.Collections.Generic;
using System.Linq;

namespace Parking.Web.Helpers
{
    public static class PlaceHelper
    {
        public static MapViewModel GetMapOccupiedPlaces(List<Place> places)
        {
            if (places.Count() == 0)
                return new MapViewModel(0,0);
            var maxColumn = places.Max(p => p.Column);
            var maxRow = places.Max(p => p.Row);
            MapViewModel mapViewModel = new MapViewModel(maxRow, maxColumn);
            foreach (var place in places)
            {
                var ispOccupiedPlace = place.CheckIns.Any();
                if (ispOccupiedPlace)
                {
                    mapViewModel.Places[place.Row - 1, place.Column - 1] = PlaceStates.Occupied;
                    continue;
                }
                mapViewModel.Places[place.Row - 1, place.Column - 1] = PlaceStates.Free;
            }
            return mapViewModel;
        }
        public static MapViewModel GetMap(List<Place> places)
        {
            if (places.Count() == 0)
                return new MapViewModel(0, 0);
            var maxColumn = places.Max(p => p.Column);
            var maxRow = places.Max(p => p.Row);
            MapViewModel mapViewModel = new MapViewModel(maxRow, maxColumn);
            foreach (var place in places)
            {
                mapViewModel.Places[place.Row - 1, place.Column - 1] = PlaceStates.Free;
            }
            return mapViewModel;
        }
    }
}
