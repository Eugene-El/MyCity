using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyCity
{
    public delegate void WeatherDelegate();

    class City
    {
        public int Width { get; }
        public int Height { get; }
        public List<Person> People { get; }
        public List<House> Houses { get; }
        private Random Rand;
        public CityMap CityMap { get; private set; }
        public Weather WeatherNow { get; private set; }

        public ObservableCollection<Person> PeopleCollection{ get { return new ObservableCollection<Person>(People); } }

        public event WeatherDelegate RainEvent;
        public event WeatherDelegate SunEvent;

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
            if (instance != null)
                return instance;

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

            WeatherNow = Weather.Sunny;
            Thread CityLife = new Thread(() => CityGeneration(peopleCount));
            CityLife.Start();
        }
        
        private void CityGeneration(int peopleCount)
        {
            CityMap = new CityMap();

            RainEvent += CityMap.MakeRainnyMap;
            SunEvent += CityMap.MakeSunnyMap;

            for (int i = 0; i < peopleCount; i++)
                AddPerson(new Person(Rand), Reason.PersonArived);

            for (int i = 0; i < 4; i++)
                Houses.Add(House.GenerateHouse(Rand));
            

            
            Live();
        }

        void Live()
        {
            Thread.Sleep(Rand.Next(10000, 60000)); // From 10 to 60 seconds

            if (Rand.Next(1, 3) == 1)
                StartRain();
            else
                MakeSunny();

            Live();
        }

        void StartRain()
        {
            if (WeatherNow != Weather.Rainy)
            {
                WeatherNow = Weather.Rainy;
                Log("Weather now: Rainy", ConsoleColor.DarkCyan);
                RainEvent?.Invoke();
            }
        }

        void MakeSunny()
        {
            if (WeatherNow != Weather.Sunny)
            {
                WeatherNow = Weather.Sunny;
                Log("Weather now: Sunny", ConsoleColor.DarkCyan);
                SunEvent?.Invoke();
            }
        }

        public void AddPerson(Person person, Reason reason)
        {
            People.Add(person);
            RainEvent += person.HideFromRain;
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
            person = null;
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
