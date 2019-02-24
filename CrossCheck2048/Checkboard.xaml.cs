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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CrossCheck2048.Game;
using CrossCheck2048.Game.Tiles;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CrossCheck2048
{
    public sealed partial class Checkboard
    {
        private readonly ITileControl _tiles;
        private bool _lockGameMoves = false;
        
        public Checkboard()
        {
            this.InitializeComponent();
            CheckboardCanvas.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 187, 173, 160));
            _tiles = GameLogic.StartGameLogic(CheckboardCanvas);
        }
        
        public void MoveDirection(DirectionEnum direction)
        {
            if (_lockGameMoves) return;
                
            if (_tiles.MoveTiles(direction))
            {
                Score.Text = _tiles.Score.ToString();
                _tiles.CreateTile();
            }

            if (_tiles.IsGameFinished())
            {
                OverlayPanel.Visibility = Visibility.Visible;
                _lockGameMoves = true;
            }
        }

        public void Reset()
        {
            if (OverlayPanel.Visibility != Visibility.Collapsed)
                OverlayPanel.Visibility = Visibility.Collapsed;
            _lockGameMoves = false;
            _tiles.Reset();

            Score.Text = _tiles.Score.ToString();
        }

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            Reset();
        }
    }
}
