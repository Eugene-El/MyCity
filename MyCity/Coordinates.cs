using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCity
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinates() : this(0,0) { }
        public Coordinates(Coordinates Coords) : this(Coords.X, Coords.Y) { }
        public Coordinates(Random rand) : this(rand.Next(City.GetInstance().Width), rand.Next(City.GetInstance().Height)) { }


        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        public bool Equals(Coordinates coords)
        {
            return (X == coords.X && Y == coords.Y);
        }

        static public Coordinates GetCoordsOnCityBorder(Random rand)
        {
            switch (rand.Next(4))
            {
                case 0:
                    return new Coordinates(rand.Next(City.GetInstance().Width), 0);

                case 1:
                    return new Coordinates(rand.Next(City.GetInstance().Width), City.GetInstance().Height - 1);

                case 2:
                    return new Coordinates(0, rand.Next(City.GetInstance().Height));

                case 3:
                    return new Coordinates(City.GetInstance().Width - 1, rand.Next(City.GetInstance().Height));
            }
            return new Coordinates();
        }
    }
}
