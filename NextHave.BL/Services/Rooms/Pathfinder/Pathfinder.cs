using Roy_T.AStar.Grids;
using Roy_T.AStar.Primitives;

namespace NextHave.BL.Services.Rooms.Pathfinder
{
    public class Pathfinder
    {
        Grid? grid;

        public Grid? GetGrid
            => grid;

        public void Initialize(int mapSizeX, int mapSizeY, bool disableDiagonal)
        {
            if (disableDiagonal)
                grid = Grid.CreateGridWithLateralConnections(new GridSize(mapSizeX, mapSizeY), new Size(Distance.FromMeters(1), Distance.FromMeters(1)), traversalVelocity: Velocity.FromKilometersPerHour(1));
            else
                grid = Grid.CreateGridWithLateralAndDiagonalConnections(new GridSize(mapSizeX, mapSizeY), new Size(Distance.FromMeters(1), Distance.FromMeters(1)), traversalVelocity: Velocity.FromKilometersPerHour(1));

            for (int x = 0; x < mapSizeX; x++)
            {
                for (int y = 0; y < mapSizeY; y++)
                {

                }
            }
        }
    }
}