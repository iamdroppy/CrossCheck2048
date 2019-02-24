using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using CrossCheck2048.Game.Tiles;

namespace CrossCheck2048.Game
{
    internal class GameEnvironment : IGame
    {
        private readonly Checkboard _gameBoard;

        private GameEnvironment(Checkboard gameBoard)
        {
            _gameBoard = gameBoard;
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Right:
                    _gameBoard.MoveDirection(DirectionEnum.Right);
                    break;
                case VirtualKey.Left:
                    _gameBoard.MoveDirection(DirectionEnum.Left);
                    break;
                case VirtualKey.Up:
                    _gameBoard.MoveDirection(DirectionEnum.Up);
                    break;
                case VirtualKey.Down:
                    _gameBoard.MoveDirection(DirectionEnum.Down);
                    break;
                case VirtualKey.R:
                    _gameBoard.Reset();
                    break;
            }
        }

        public static IGame CreateGame(Checkboard gameBoard)
        {
            return new GameEnvironment(gameBoard);
        }

        public void StartGame()
        {
            CoreWindow.GetForCurrentThread().KeyDown += OnKeyDown;
        }

        public void StopGame()
        {
            CoreWindow.GetForCurrentThread().KeyDown -= OnKeyDown;
        }
    }
}
