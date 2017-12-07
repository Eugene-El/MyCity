using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyCity
{
    class City
    {
        public int Width { get; }
        public int Height { get; }
        public List<Person> People { get; set; }
        public List<House> Houses { get; set; }
        private Random Rand;
        public CityMap CityMap { get; private set; }

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
            CityMap = new CityMap();

            for (int i = 0; i < peopleCount; i++)
                AddPerson(new Person(Rand), Reason.PersonArived);

            for (int i = 0; i < 4; i++)
                Houses.Add(House.GenerateHouse(Rand));
            Draw();

            
            Live();
        }

        void Live()
        {
            Thread.Sleep(10000);

            // TODO write city live code 
            //Draw();

            Live();
        }

        public void AddPerson(Person person, Reason reason)
        {
            People.Add(person);
            person.PersonBorn += AddPerson;
            person.PersonInfo += Log;
            person.PersonBuildHouse += AddHouse;
            person.PersonDie += DeletePerson;
            Log(person + (reason == Reason.PersonBorned ? " was born!" : " arrived to the city!"), 
                reason == Reason.PersonBorned ? ConsoleColor.Yellow : ConsoleColor.DarkYellow);
        }

        public void DeletePerson(Person person)
        {
            People.Remove(person);
            Log(person + " died!", ConsoleColor.Red);
        }

        public void AddHouse(House house)
        {
            Houses.Add(house);
            Log("House bilded at " + house.Coords + " !", ConsoleColor.Green);
        }

        public void Log(string info, ConsoleColor cc)
        {
            Console.ForegroundColor = cc;
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + info);
        }

        public void Draw()
        {
            CityMap.Draw();
            CityMap.Save();
            Log("IMAGE", ConsoleColor.Cyan);
        }

    }
}
