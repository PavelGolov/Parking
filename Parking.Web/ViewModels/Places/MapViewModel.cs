using Parking.Core.Enums;

namespace Parking.Web.ViewModels.Places
{
    public class MapViewModel
    {
        public PlaceStates[,] Places { get; set; }
        public MapViewModel(int maxRow, int maxColumn)
        {
            Places = new PlaceStates[maxRow, maxColumn];
            for (int i = 0; i<maxRow; i++)
            {
                for (int j = 0; j < maxColumn; j++)
                {
                    Places[i, j] = PlaceStates.None;
                }
            }
        }

    }
}
