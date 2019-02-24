using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CrossCheck2048.Game;
using CrossCheck2048.Game.Tiles;

namespace CrossCheck2048
{
    public delegate void OnPositionChanged();
    public sealed partial class Tile : IDisposable
    {
        private int _points;

        /// <summary>
        /// Current Tile Position.
        /// </summary>
        public TileVector Position { get; }

        /// <summary>
        /// Current points from this tile.
        /// </summary>
        public int Points
        {
            get => _points;
            private set
            {
                _points = value;
                SetBackground();
                UpdatePosition();
            }
        }

        public Tile(int x, int y, int points)
        {
            InitializeComponent();
            Position = new TileVector(x, y);
            Position.PositionChanged += OnPositionChanged;
            Points = points;
            UpdatePosition();
            SetBackground();
        }

        private void OnPositionChanged()
        {
            UpdatePosition();
        }

        /// <summary>
        /// Merge Tiles
        /// </summary>
        /// <param name="other">The other tile</param>
        public void MergeWith(Tile other)
        {
            Points = Points * 2;
            other.Position.CopyTo(Position);
            UpdatePosition();
        }

        /// <summary>
        /// This will set the colors accordingly.
        /// I have copied the colors from another project.
        /// </summary>
        public void SetBackground()
        {
            switch (Points)
            {
                case 2: TileGrid.Background = new SolidColorBrush(Color.FromArgb(240, 240, 230, 220)); break;
                case 4: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 240, 224, 200)); break;
                case 8: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 240, 175, 120)); break;
                case 16: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 245, 150, 100)); break;
                case 32: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 250, 125, 90)); break;
                case 64: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 245, 95, 60)); break;
                case 128: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 235, 210, 115)); break;
                case 256: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 235, 205, 100)); break;
                case 512: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 235, 200, 80)); break;
                case 1024: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 235, 200, 60)); break;
                case 2048: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 235, 195, 50)); break;
                case 4096: TileGrid.Background = new SolidColorBrush(Color.FromArgb(255, 240, 195, 30)); break;
                default: TileGrid.Background = new SolidColorBrush(Colors.DarkGray); break;

            }
        }

        /// <summary>
        /// Updates the position and updates the screen
        /// </summary>
        private void UpdatePosition()
        {
            points.Text = Points.ToString();
            Render();
            UpdateLayout();
        }

        /// <summary>
        /// Render based on position.
        /// </summary>
        public void Render()
        {
            Margin = new Thickness(Position.X * 100, Position.Y * 100, 0, 0);
        }

        /// <summary>
        /// Dispose and rids away from this event.
        /// </summary>
        public void Dispose()
        {
            Position.PositionChanged -= OnPositionChanged;
        }
    }
}
