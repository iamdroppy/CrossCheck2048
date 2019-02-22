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

        public Tile GetClosestTileFromDirection(Tile tile, DirectionEnum direction)
        {
            if (direction == DirectionEnum.Left || direction == DirectionEnum.Right)
                return GetTilesFromDirection(direction).SkipWhile(s => s != tile).FirstOrDefault(s => s != tile && s.Position.Y == tile.Position.Y);
            else
                return GetTilesFromDirection(direction).SkipWhile(s => s != tile).FirstOrDefault(s => s != tile && s.Position.X == tile.Position.X);
        }

        public void MoveFarthestAvailableColumn(Tile tile, DirectionEnum direction)
        {
            Tile closestTileOnDirection = GetClosestTileFromDirection(tile, direction);

            if (closestTileOnDirection != null)
            {
                if (closestTileOnDirection.Points == tile.Points)
                {
                    tile.MergeWith(closestTileOnDirection);
                    tile.MoveTo(closestTileOnDirection.Position);
                    Tiles.Remove(closestTileOnDirection);
                    _canvas.Children.Remove(closestTileOnDirection);
                }
                else
                {
                    if (direction == DirectionEnum.Left)
                        tile.Position.X = closestTileOnDirection.Position.X - 1;
                    else if (direction == DirectionEnum.Right)
                        tile.Position.X = closestTileOnDirection.Position.X + 1;
                    else if (direction == DirectionEnum.Down)
                        tile.Position.Y = closestTileOnDirection.Position.Y + 1;
                    else if (direction == DirectionEnum.Up)
                        tile.Position.Y = closestTileOnDirection.Position.Y - 1;

                    tile.MoveTo(tile.Position);
                }
            }
            else
            {
                if (direction == DirectionEnum.Left)
                    tile.Position.X = 0;
                else if (direction == DirectionEnum.Right)
                    tile.Position.X = 3;
                else if (direction == DirectionEnum.Up)
                    tile.Position.Y = 0;
                else if (direction == DirectionEnum.Down)
                    tile.Position.Y = 3;

                tile.MoveTo(tile.Position);
            }

            _canvas.UpdateLayout();
        }

        public void MoveTiles(DirectionEnum direction)
        {
            foreach (var tile in GetTilesFromDirection(direction))
            {
                MoveFarthestAvailableColumn(tile, direction);
            }
        }

        private int temp = 0;
        public void CreateTile()
        {
            Tile tile = new Tile(new TileVector(temp, temp));
            _canvas.Children.Add(tile);
            Tiles.Add(tile);
        }

        private IEnumerable<Tile> GetTilesFromDirection(DirectionEnum direction)
        {
            if (direction == DirectionEnum.Left || direction == DirectionEnum.Up)
            {
                for (int x = 0; x <= 3; x++)
                {
                    for (int y = 0; y <= 3; y++)
                        if (HasTile(x, y))
                            yield return GetTile(x, y);
                }
            }
            else
            {
                for (int x = 3; x >= 0; x--)
                {
                    for (int y = 3; y >= 0; y--)
                        if (HasTile(x, y))
                            yield return GetTile(x, y);
                }
            }
        }

        public static ITileControl CreateTileControl(Canvas canvas)
        {
            return new TileControl(canvas);
        }
    }
}