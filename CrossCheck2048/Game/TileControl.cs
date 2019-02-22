using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CrossCheck2048.Game
{
    public interface ITileControl
    {
        bool HasTile(int x, int y);
        void MoveTiles(DirectionEnum direction);
        void CreateTile();
    }
    public class TileControl : ITileControl
    {
        private Random _random = new Random();
        private readonly Canvas _canvas;
        public List<Tile> Tiles { get; }

        private TileControl(Canvas canvas)
        {
            Tiles = new List<Tile>();
            _canvas = canvas;
        }

        public bool HasTile(int x, int y)
        {
            return Tiles.Any(s => s.Position.X == x && s.Position.Y == y);
        }

        public Tile GetTile(int x, int y)
        {
            return Tiles.FirstOrDefault(s => s.Position.X == x && s.Position.Y == y);
        }

        public TileVector GetRandomTile()
        {
            var tiles = GetFreeTiles().ToArray();
            return tiles.ElementAt(_random.Next(0, tiles.Count()));
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

        public Tile GetClosestTileFromDirection(Tile tile, DirectionEnum direction)
        {
            if (direction == DirectionEnum.Left || direction == DirectionEnum.Right)
                return GetTilesFromDirection(direction).SkipWhile(s => s != tile).FirstOrDefault(s => s != tile && s.Position.Y == tile.Position.Y);
            else
                return GetTilesFromDirection(direction).SkipWhile(s => s != tile).FirstOrDefault(s => s != tile && s.Position.X == tile.Position.X);
        }

        public bool MoveFarthestAvailableColumn(Tile tile, DirectionEnum direction, bool canMerge = true)
        {
            Tile closestTileOnDirection = GetClosestTileFromDirection(tile, direction);
            bool hasMoved = false;
            if (closestTileOnDirection != null)
            {
                if (closestTileOnDirection.Points == tile.Points && canMerge)
                {
                    tile.MergeWith(closestTileOnDirection);
                    tile.MoveTo(closestTileOnDirection.Position);
                    Tiles.Remove(closestTileOnDirection);
                    _canvas.Children.Remove(closestTileOnDirection);
                    hasMoved = true;
                }
                else
                {
                    if (direction == DirectionEnum.Left)
                    {
                        if (closestTileOnDirection.Position.X + 1 != tile.Position.X)
                        {
                            tile.Position.X = closestTileOnDirection.Position.X + 1;
                            tile.MoveTo(tile.Position);

                            hasMoved = true;
                        }
                    }
                    else if (direction == DirectionEnum.Right)
                    {
                        if (closestTileOnDirection.Position.X - 1 != tile.Position.X)
                        {
                            tile.Position.X = closestTileOnDirection.Position.X - 1;
                            tile.MoveTo(tile.Position);

                            hasMoved = true;
                        }
                    }
                    else if (direction == DirectionEnum.Down)
                    {
                        if (closestTileOnDirection.Position.Y - 1 != tile.Position.Y)
                        {
                            tile.Position.Y = closestTileOnDirection.Position.Y - 1;
                            tile.MoveTo(tile.Position);

                            hasMoved = true;
                        }
                    }
                    else if (direction == DirectionEnum.Up)
                    {
                        if (closestTileOnDirection.Position.Y + 1 != tile.Position.Y)
                        {
                            tile.Position.Y = closestTileOnDirection.Position.Y + 1;
                            tile.MoveTo(tile.Position);
                            hasMoved = true;
                        }
                    }
                }
            }
            else
            {
                if (direction == DirectionEnum.Left && tile.Position.X != 0)
                {
                    tile.Position.X = 0;
                    hasMoved = true;
                    tile.MoveTo(tile.Position);
                }
                else if (direction == DirectionEnum.Right && tile.Position.X != 3)
                {
                    tile.Position.X = 3;
                    hasMoved = true;
                    tile.MoveTo(tile.Position);
                }
                else if (direction == DirectionEnum.Up && tile.Position.Y != 0)
                {
                    tile.Position.Y = 0;
                    hasMoved = true;
                    tile.MoveTo(tile.Position);
                }
                else if (direction == DirectionEnum.Down && tile.Position.Y != 3)
                {
                    tile.Position.Y = 3;
                    hasMoved = true;
                    tile.MoveTo(tile.Position);
                }
            }

            if (hasMoved)
                _canvas.UpdateLayout();

            return hasMoved;
        }

        public void MoveTiles(DirectionEnum direction)
        {
            bool canMove = true;
            bool canMerge = true;
            do
            {
                bool madeAnyMovementsOrMerges = false;
                foreach (var tile in GetTilesFromDirection(direction))
                {
                    bool hasMoved = MoveFarthestAvailableColumn(tile, direction, canMerge);
                    if (hasMoved)
                        madeAnyMovementsOrMerges = true;
                }


                if (!madeAnyMovementsOrMerges)
                    canMove = false;
                canMerge = false;
            } while (canMove);
        }
        
        public void CreateTile()
        {
            Tile tile = new Tile(GetRandomTile());
            _canvas.Children.Add(tile);
            Tiles.Add(tile);
        }

        private IEnumerable<Tile> GetTilesFromDirection(DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                    return Tiles.OrderByDescending(s => s.Position.X);
                case DirectionEnum.Right:
                    return Tiles.OrderBy(s => s.Position.X);
                case DirectionEnum.Up:
                    return Tiles.OrderByDescending(s => s.Position.Y);
                case DirectionEnum.Down:
                    return Tiles.OrderBy(s => s.Position.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static ITileControl CreateTileControl(Canvas canvas)
        {
            return new TileControl(canvas);
        }
    }
}