using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CrossCheck2048.Game;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CrossCheck2048
{
    public sealed partial class Checkboard : UserControl
    {
        private readonly Random _random = new Random();
        private readonly ITileControl _tiles;

        public Checkboard()
        {
            this.InitializeComponent();
            CheckboardCanvas.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 187, 173, 160));
            _tiles = TileControl.CreateTileControl(CheckboardCanvas);
        }
        
        public void RemoveTiles(IEnumerable<Tile> tiles)
        {
            foreach (Tile tile in tiles)
                CheckboardCanvas.Children.Remove(tile);
        }

        public void MoveDirection(DirectionEnum direction)
        {
            _tiles.MoveTiles(direction);
            CreateRandomTile();
        }

        //public void RemoveTile(Tile tile)
        //{
        //    _tiles.Remove(tile);
        //    CheckboardCanvas.Children.Remove(tile);
        //}
        public void CreateRandomTile()
        {
            _tiles.CreateTile();
        }
    }
}
