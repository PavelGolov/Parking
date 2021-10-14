using Parking.Core.Enums;
using Parking.SharedKernel.Models;
using System.Collections.Generic;
using System.Linq;

namespace Parking.Core.Factories
{
    public class PlaceMapFactory
    {
        public PlaceStates[,] CreateOccupiedPlaceMap(List<Place> places)
        {
            var placeMap = GetEmptyMap(places);

            foreach (var place in places)
            {
                var ispOccupiedPlace = place.CheckIns.Any();
                if (ispOccupiedPlace)
                {
                    placeMap[place.Row - 1, place.Column - 1] = PlaceStates.Occupied;
                    continue;
                }
                placeMap[place.Row - 1, place.Column - 1] = PlaceStates.Free;
            }
            return placeMap;
        }
        public PlaceStates[,] CreatePlaceMap(List<Place> places)
        {
            var placeMap = GetEmptyMap(places);

            foreach (var place in places)
            {
                placeMap[place.Row - 1, place.Column - 1] = PlaceStates.Free;
            }

            return placeMap;
        }

        private PlaceStates[,] GetEmptyMap(List<Place> places)
        {
            if (places.Count() == 0)
                return new PlaceStates[0, 0];

            var maxColumn = places.Max(p => p.Column);
            var maxRow = places.Max(p => p.Row);

            var placeMap = new PlaceStates[maxRow, maxColumn];

            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxColumn; j++)
                {
                    placeMap[i, j] = PlaceStates.None;
                }
            }

            return placeMap;
        }
    }
}
