using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCity
{
    class House
    {
        public Coordinates Coords { get; }
        public int Height { get; }
        public int Width { get; } 

        public List<int> LodgersList { get; }

        public bool IsEmpty
        {
            get
            {
                return LodgersList.Count == 0;
            }
        }

        public House(Coordinates coords, int width, int height)
        {
            LodgersList = new List<int>();   
            Coords = coords;
            Height = height;
            Width = width;
        }

        public House(int x, int y, int width, int height) : this(new Coordinates(x, y), width, height) { }

        public House(Random rand) : this(new Coordinates(rand), rand.Next(2, 6), rand.Next(2, 6)) { }


        // // // //

        public static House GenerateHouse(Random rand)
        {
            House house;
            bool isIncorrect;
            do
            {
                house = new House(rand);
                isIncorrect = false;
                if ((house.Coords.X + house.Width > City.GetInstance().Width || house.Coords.Y + house.Height > City.GetInstance().Height)) // House on border of city check
                    isIncorrect = true;

                if (!isIncorrect)
                    isIncorrect = !CanCreateInCity(house);
                
            } while (isIncorrect);

            return house;
        }

        public static bool CanCreateInCity(House house)
        {
            foreach (House h in City.GetInstance().Houses)
            {
                if (h.CheckForIntersection(house))
                    return false;
            }
            return true;
        }

        public bool CheckForIntersection(House house)
        {
            return (checkCoordinatesInHouse(house.Coords) || // LU 
                checkCoordinatesInHouse(new Coordinates(house.Coords.X + house.Width, house.Coords.Y)) || // RU
                checkCoordinatesInHouse(new Coordinates(house.Coords.X, house.Coords.Y + house.Height)) || // LL
                checkCoordinatesInHouse(new Coordinates(house.Coords.X + house.Width, house.Coords.Y + house.Height)) || // RL
                ((house.Coords.X >= Coords.X - 1 && house.Coords.X <= Coords.X + Width + 1 && house.Coords.X + house.Width >= Coords.X - 1 && house.Coords.X + house.Width <= Coords.X + Width + 1) &&
                (house.Coords.Y >= Coords.Y - 1 && house.Coords.Y <= Coords.Y + Height + 1 && house.Coords.Y + house.Height >= Coords.Y - 1 && house.Coords.Y + house.Height <= Coords.Y + Height + 1)));
        }

        private bool checkCoordinatesInHouse(Coordinates coords)
        {
            return (coords.X >= Coords.X - 1 && coords.X <= Coords.X + Width + 1 &&
                coords.Y >= Coords.Y - 1 && coords.Y <= Coords.Y + Height + 1);
        }

        public void Draw()
        {
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    if (j == 0 || j  == Height-1 || i == 0 || i == Width - 1)
                        City.GetInstance().CityMap.SetPixel(Coords.X+i, Coords.Y+j, Color.Red);
                    else
                        City.GetInstance().CityMap.SetPixel(Coords.X + i, Coords.Y + j, Color.Yellow);
        }
    }
}
