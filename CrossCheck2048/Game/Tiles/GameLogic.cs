using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CrossCheck2048.Game.Tiles
{
    /// <summary>
    /// Tile control logic and general game logic.
    /// We are mostly using LINQ for operations. Keep a note that most queries are deferred (not immediate), so these queries can build up with other queries.
    /// Optimally, the tile control class wouldn't be the same as the logic one, and this class should be smaller, and the queries should be stored somewhere else.
    /// </summary>
    public class GameLogic : ITileControl
    {
        /// <summary>
        /// Random for points and random tiles.
        /// </summary>
        private readonly Random _random = new Random();
        
        /// <summary>
        /// In-game checkboard.
        /// </summary>
        private readonly Canvas _checkboardCanvas;

        /// <summary>
        /// Playable tiles.
        /// Capacity: 16 (number of possible tiles).
        /// </summary>
        public List<Tile> Tiles { get; }

        /// <summary>
        /// In-Game score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Constructor passes canvas since removed tiles must also be removed from canvas.
        /// </summary>
        /// <param name="checkboardCanvas">Checkboard Canvas</param>
        private GameLogic(Canvas checkboardCanvas)
        {
            Tiles = new List<Tile>(16);
            _checkboardCanvas = checkboardCanvas;

            CreateTile();
            CreateTile();
        }

        /// <summary>
        /// Verifies if there's any tile in the given position.
        /// </summary>
        /// <param name="x">Tile X</param>
        /// <param name="y">Tile Y</param>
        /// <returns></returns>
        public bool HasTile(int x, int y) => Tiles.Any(s => s.Position.X == x && s.Position.Y == y);

        /// <summary>
        /// Creates a random tile. Points are either 2 or 4.
        /// </summary>
        public void CreateTile()
        {
            var tileVectors = GetFreeTiles().ToArray(); // since we're doing multiple operations with it, we use an array: we don't want to execute many immediate queries on this one.

            if (!tileVectors.Any()) return; // if there's no free tiles, return (don't create a new tile).
            var vector = tileVectors.RandomElement(_random); // gets a random tile (passing Random, we could initialize Random in there, but the RNG is not as safe this way).
            var tile = new Tile(vector.X, vector.Y, _random.Next(0, 2) == 0 ? 2 : 4); // creates a new tile in a given vector
            ;
            _checkboardCanvas.Children.Add(tile); // adds to the checkboard.
            Tiles.Add(tile); // adds to the playable tiles list.
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void Reset()
        {
            Score = 0;
            _checkboardCanvas.Children.Clear();

            foreach (var tile in Tiles)
                tile.Dispose(); // we must loop/dispose here to remove events declared on the Tile class.
            Tiles.Clear();
            CreateTile();
            CreateTile();
        }

        /// <summary>
        /// Verifies if the game is finished.
        /// </summary>
        /// <returns>Either the game is finished or not.</returns>
        public bool IsGameFinished()
        {
            if (Tiles.Count != 16) return false; // if tiles count isn't 16, then the game isn't finished.

            // this is a fairly big operation, it executes fairly quickly though, so in this case I don't see the need for optimization.
            // for every tile, it will see the tiles that are in its range (Y +/- and X +/-). If there's any one that can be mergeable, then returns false, saying the game isn't finished.
            foreach (var tile in Tiles)
            {
                var canMerge = Tiles.Where(s => tile != s && s.Position.IsInRange(tile.Position)).Any(s => s.Points == tile.Points);

                if (canMerge)
                    return false;
            }

            return true;

        }
        
        /// <summary>
        /// This operation gets the closest tile from direction.
        /// We must consider the direction for the merging possibility.
        /// </summary>
        /// <param name="tile">Current tile that we are analysing</param>
        /// <param name="direction">Direction of the movement.</param>
        /// <returns></returns>
        public Tile GetClosestTileFromDirection(Tile tile, DirectionEnum direction)
        {
            var tiles = GetTilesFromDirection(direction).Reverse().SkipWhile(s => s != tile);
            if (direction == DirectionEnum.Left || direction == DirectionEnum.Right)
                return tiles.FirstOrDefault(s => s != tile && s.Position.Y == tile.Position.Y);
            else
                return tiles.FirstOrDefault(s => s != tile && s.Position.X == tile.Position.X);
        }

        /// <summary>
        /// This will move to the next available column.
        /// </summary>
        /// <param name="tile">Current tile</param>
        /// <param name="direction">Which direction we're headed to.</param>
        /// <param name="canMerge">Can Merge is only true on its first iteration, so it prevents from merging multiple times.</param>
        /// <returns></returns>
        public bool MoveFarthestAvailableColumn(Tile tile, DirectionEnum direction, bool canMerge = true)
        {
            Tile closestTileOnDirection = GetClosestTileFromDirection(tile, direction); // gets the closest tile that we are headed.

            if (closestTileOnDirection == null) // if it's null, then we move all the way through direction (either X:0, X:3, Y:0, Y:3)
                return MoveTileThroughDirection(tile, direction);

            if (closestTileOnDirection.Points != tile.Points || !canMerge) // if you can't merge with the next tile
                return MoveTileNextToOtherTile(tile, closestTileOnDirection, direction);

            MergeTiles(tile, closestTileOnDirection); // merges tiles
            return true;

        }

        /// <summary>
        /// Merge two tiles together
        /// </summary>
        /// <param name="from">Current tile</param>
        /// <param name="to">Tile to be </param>
        public void MergeTiles(Tile @from, Tile to)
        {
            Score += @from.Points * 2; // adds to the score.
            to.MergeWith(@from);
            Tiles.Remove(@from);
            @from.Dispose(); // to remove listeners.
            _checkboardCanvas.Children.Remove(@from);
        }

        /// <summary>
        /// Moves tile to otherTile following the current direction.
        /// </summary>
        /// <param name="tile">Current tile</param>
        /// <param name="otherTile">Next tile</param>
        /// <param name="direction">Direction</param>
        /// <returns>If tile was moved.</returns>
        public bool MoveTileNextToOtherTile(Tile tile, Tile otherTile, DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left when otherTile.Position.X + 1 != tile.Position.X:
                    tile.Position.X = otherTile.Position.X + 1;
                    return true;
                case DirectionEnum.Right when otherTile.Position.X - 1 != tile.Position.X:
                    tile.Position.X = otherTile.Position.X - 1;
                    return true;
                case DirectionEnum.Down when otherTile.Position.Y - 1 != tile.Position.Y:
                    tile.Position.Y = otherTile.Position.Y - 1;
                    return true;
                case DirectionEnum.Up when otherTile.Position.Y + 1 != tile.Position.Y:
                    tile.Position.Y = otherTile.Position.Y + 1;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Moves tile to the edges.
        /// </summary>
        /// <param name="tile">Current tile</param>
        /// <param name="direction">Direction we're headed.</param>
        /// <returns>If tile was moved.</returns>
        public bool MoveTileThroughDirection(Tile tile, DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left when tile.Position.X != 0:
                    tile.Position.X = 0;
                    return true;
                case DirectionEnum.Right when tile.Position.X != 3:
                    tile.Position.X = 3;
                    return true;
                case DirectionEnum.Up when tile.Position.Y != 0:
                    tile.Position.Y = 0;
                    return true;
                case DirectionEnum.Down when tile.Position.Y != 3:
                    tile.Position.Y = 3;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// MoveTiles towards direction.
        /// </summary>
        public bool MoveTiles(DirectionEnum direction)
        {
            bool canMerge = true;
            int movements = 0;

            // calls the method to see if the tiles can move and if they did move we will increment movements, and after the first iteration, it won't be able to move anymore.
            while (MoveTiles(direction, canMerge))
            {
                movements++;
                canMerge = false;
            }

            return movements > 0;
        }

        /// <summary>
        /// Move tiles once to a specific direction.
        /// </summary>
        private bool MoveTiles(DirectionEnum direction, bool canMerge)
        {
            // this query is straight forward.
            // Gets the tile from direction, move to farthest available column (with or without merge) and do a count of how many tiles moved.
            // we are using Count(*) instead of Any(*) here because LINQ will translate to the first that is true and won't move the rest.
            return GetTilesFromDirection(direction).Select(tile => MoveFarthestAvailableColumn(tile, direction, canMerge)).Count(s => s == true) > 0;
        }

        public IEnumerable<TileVector> GetFreeTiles()
        {
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 0; y <= 3; y++)
                {
                    if (!HasTile(x, y))
                        yield return new TileVector(x, y);
                }
            }
        }

        /// <summary>
        /// We get the tiles ordered by from the direction we are headed. We can't always use linear in this case.
        /// </summary>
        /// <param name="direction">Direction we're headed</param>
        /// <returns>Tiles ordered by Direction</returns>
        private IEnumerable<Tile> GetTilesFromDirection(DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                    return Tiles.OrderBy(s => s.Position.X);
                case DirectionEnum.Right:
                    return Tiles.OrderByDescending(s => s.Position.X);
                case DirectionEnum.Up:
                    return Tiles.OrderBy(s => s.Position.Y);
                case DirectionEnum.Down:
                    return Tiles.OrderByDescending(s => s.Position.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        /// <summary>
        /// Create tile, and just give the requester the interface.
        /// </summary>
        public static ITileControl StartGameLogic(Canvas canvas)
        {
            return new GameLogic(canvas);
        }
    }
}