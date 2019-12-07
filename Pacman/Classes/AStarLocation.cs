using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman.Classes
{
    class AStarLocation
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public AStarLocation Parent;
    }
}
