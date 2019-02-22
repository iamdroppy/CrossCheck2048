using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace CrossCheck2048
{
    public sealed partial class Tile : UserControl
    {
        public TileVector Position { get; private set; }
        public int Points { get; private set; }

        public Tile(TileVector position)
        {
            InitializeComponent();

            Position = position;
            Points = 2;
            UpdatePosition();
        }

        public void MergeWith(Tile other)
        {
            Points = Points * 2;
        }

        private void UpdatePosition()
        {
            points.Text = Points.ToString();
            Render();
            UpdateLayout();
        }

        public void Render()
        {
            Margin = new Thickness(Position.X * 100, Position.Y * 100, 0, 0);
        }

        public void MoveTo(DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Left:
                    Position.X--;
                    break;
                case DirectionEnum.Right:
                    Position.X++;
                    break;
                case DirectionEnum.Up:
                    Position.Y--;
                    break;
                case DirectionEnum.Down:
                    Position.Y++;
                    break;
            }

            UpdatePosition();
        }

        public void MoveTo(TileVector tile)
        {
            Position = tile.Clone();
            UpdatePosition();
        }
    }
}
