using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCheck2048.Game.Tiles
{
    public class TileVector
    {
        /// <summary>
        /// Event to when a position is changed.
        /// </summary>
        public event OnPositionChanged PositionChanged;

        private int _x;
        private int _y;
        public TileVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X
        {
            get => _x;
            set
            {
                if (value == _x) return;
                if (value > 3)
                    value = 3;
                else if (value < 0)
                    value = 0;

                _x = value;

                PositionChanged?.Invoke();
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                if (value == _y) return;
                if (value > 3)
                    value = 3;
                else if (value < 0)
                    value = 0;

                _y = value;

                PositionChanged?.Invoke();
            }
        }

        public bool IsInRange(TileVector vector)
        {
            return (X == vector.X && (vector.Y == Y + 1 || vector.Y == Y - 1) ||
                    Y == vector.Y && (vector.X == X + 1 || vector.X == X - 1));
        }

        public void CopyTo(TileVector vector)
        {
            vector.X = _x;
            vector.Y = _y;

            PositionChanged?.Invoke();
        }
    }
}
