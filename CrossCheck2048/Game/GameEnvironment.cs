using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace CrossCheck2048.Game
{
    internal class GameEnvironment : IGame
    {
        private readonly Checkboard _gameBoard;

        private GameEnvironment(Checkboard gameBoard)
        {
            _gameBoard = gameBoard;
            CoreWindow.GetForCurrentThread().KeyDown += OnKeyDown;
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            DirectionEnum direction = DirectionEnum.Down;

            switch (args.VirtualKey)
            {
                case VirtualKey.Right:
                    direction = DirectionEnum.Right;
                    break;
                case VirtualKey.Left:
                    direction = DirectionEnum.Left;
                    break;
                case VirtualKey.Up:
                    direction = DirectionEnum.Up;
                    break;
                case VirtualKey.Down:
                    direction = DirectionEnum.Down;
                    break;
                case VirtualKey.A:
                    _gameBoard.CreateRandomTile();
                    return;
                default:
                    return;
            }


            _gameBoard.MoveDirection(direction);
        }

        public static IGame CreateGame(Checkboard gameBoard)
        {
            return new GameEnvironment(gameBoard);
        }

        public async Task StartGame()
        {
            await RenderCheckboard();
        }
        
        public Task StopGame()
        {
            return Task.CompletedTask;
        }

        private async Task RenderCheckboard()
        {
            _gameBoard.CreateRandomTile();
            _gameBoard.CreateRandomTile();
        }
    }
}
