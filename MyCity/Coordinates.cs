using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCity
{
    class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinates() : this(0,0) { }

        public Coordinates(Random rand)
        {
            X = rand.Next(City.GetInstance().Width);
            Y = rand.Next(City.GetInstance().Height);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        public bool Equals(Coordinates coords)
        {
            return (X == coords.X && Y == coords.Y);
        }
    }
}
