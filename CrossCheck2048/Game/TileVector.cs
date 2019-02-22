using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCheck2048.Game
{
    public class TileVector
    {
        private int _x;
        private int _y;
        public int X
        {
            get => _x;
            set
            {
                if (value > 3)
                    value = 3;
                else if (value < 0)
                    value = 0;

                _x = value;
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                if (value > 3)
                    value = 3;
                else if (value < 0)
                    value = 0;

                _y = value;
            }
        }

        public TileVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public TileVector Clone()
        {
            return new TileVector(X, Y);
        }

        public static bool IsValidVector(int x, int y)
        {
            return (x >= 0 && x <= 3) && (y >= 0 && y <= 3);
        }
    }
}
