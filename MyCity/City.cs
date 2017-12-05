using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCity
{
    class City
    {
        public int Width { get; }
        public int Height { get; }
        public List<Person> People { get; set; }
        public List<House> Houses { get; set; }
        private Random Rand;


        // Singleton
        private static City instance;

        public static City GetInstance(int width, int height, int peopleCount)
        {
            if (instance == null)
                instance = new City(width, height, peopleCount);
            return instance;
        }

        public static City GetInstance()
        {
            Random r = new Random();
            return GetInstance(r.Next(1000), r.Next(1000), r.Next(19) + 1);
        }
        //

        private City(int width, int height, int peopleCount)
        {
            Width = width;
            Height = height;

            People = new List<Person>();
            Houses = new List<House>();
            Rand = new Random();




            Thread CityLife = new Thread(() => CityGeneration(peopleCount));
            CityLife.Start();
        }

        private void CityGeneration(int peopleCount)
        {
            for (int i = 0; i < peopleCount; i++)
                AddPerson(new Person(Rand), Reason.PersonArived);

            for (int i = 0; i < 4; i++)
                Houses.Add(House.GenerateHouse(Rand));
            Draw();

            Live();
        }

        void Live()
        {
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Green;
            Log("Tick");
            Live();
        }

        public void AddPerson(Person person, Reason reason)
        {
            People.Add(person);
            person.PersonBorn += AddPerson;
            person.PersonInfo += Log;
            person.PersonDie += DeletePerson;
            Console.ForegroundColor = reason == Reason.PersonBorned ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
            Log(person + (reason == Reason.PersonBorned ? " was born!" : " arrived to the city!"));
        }

        public void DeletePerson(Person person)
        {
            People.Remove(person);
            Console.ForegroundColor = ConsoleColor.Red;
            Log(person + " died!");
        }

        public void Log(string info)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + info);
        }

        public void Draw()
        {
            Bitmap b = new Bitmap(Width, Height);
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                    b.SetPixel(i, j, Color.Lime);
            foreach (House h in Houses)
                h.Draw(b);
            b.Save("City.png");
            Log("IMAGE");
        }

    }
}
