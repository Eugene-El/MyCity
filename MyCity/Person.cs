﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCity
{
    delegate void PersonBornDelegate(Person person, Reason reason);
    delegate void PersonInfoDelegate(string str, ConsoleColor cc);
    delegate void PersonDieDelegate(Person person);
    delegate void PersonBuildHouseDelegate(House house);

    class Person
    {
        private static int currentID = 1;
        private static string[] names = { "John", "Jack", "Luis", "Eugene", "Kirill", "Svetlana", "Ann", "Boris", "Denis", "Poll", "Serge", "Mark", "Ricardo", "Tina", "Leo", "Anton" };
        private static string[] surnames = { "Smith", "Holand", "Rufalo", "Alvarez", "Mango", "Cant", "Huff" };
        private Random Rand;
        public int ID { get; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string FullName { get { return Name + " " + Surname; } }
        public DateTime Birthday { get; }
        public Coordinates Coords { get; set; }

        public bool HaveHouse
        {
            get
            {
                bool result = false;
                for (int i = 0; i < City.GetInstance().Houses.Count; i++)
                    if (City.GetInstance().Houses[i].LodgersList.Contains(ID))
                        result = true;
                return result;
            }
        }

        public Coordinates HouseCoords
        {
            get
            {
                if (HaveHouse)
                    return City.GetInstance().Houses.Find(h => h.LodgersList.Contains(ID)).Coords;
                return null;
            }
        }

        public bool AtHome { get { return Coords.Equals(HouseCoords); } }

        // Targeting
        public Coordinates Target { get; private set; }
        private bool GoToHome;
        //

        public event PersonBornDelegate PersonBorn;
        public event PersonInfoDelegate PersonInfo;
        public event PersonDieDelegate PersonDie;
        public event PersonBuildHouseDelegate PersonBuildHouse;

        // Live thread
        private Thread LiveThread;

        public Person(string name, string surname, Random rand)
        {
            ID = currentID++;
            Name = name;
            Surname = surname;
            Rand = rand;
            Birthday = DateTime.Now;
            Coords = new Coordinates();

            GoToHome = false;

            LiveThread = new Thread(Live);
            LiveThread.Start();
        }

        public Person(Random rand)
        {
            ID = currentID++;
            Rand = rand;
            Birthday = DateTime.Now;
            Name = ChooseName();
            Surname = surnames[Rand.Next(surnames.Length)];
            Coords = Coordinates.GetCoordsOnCityBorder(Rand);

            GoToHome = false;

            LiveThread = new Thread(Live);
            LiveThread.Start();

        }

        ~Person()
        {
            Console.ResetColor();
            Console.WriteLine("----------------------- " + this + "DESTRUCTURED -----------------------");
        }

        void Live()
        {
            if (GoToHome)
            {
                //GiveInfo(this + " use GOtoHOME", ConsoleColor.DarkGray);
                GoHome();
                GoToHome = false;
            }

            

            Thread.Sleep(1000); // Every second
            if (Rand.Next(60) == 0)
            {
                int choose = Rand.Next(100);
                if (City.GetInstance().WeatherNow != Weather.Rainy)
                {
                    if (choose < 20)
                    {
                        BornPerson();
                    }
                    else if (choose < 40)
                    {
                        BuildHouse();
                    }
                    else if (choose < 60)
                    {
                        Target = new Coordinates(Rand);

                        GiveInfo(this + " go to " + Target, ConsoleColor.Blue);
                        Travel();
                    }
                    else if (choose > 95)
                    {
                        Die();
                        return;
                    }
                    else
                    {
                        GoHome();
                    }
                }
                else
                {
                    if (!AtHome)
                        GoHome();
                }
            }
            Live();
        }

        void Travel()
        {
            Thread.Sleep(500); // One speed for all

            //if (Target == null) return;
            
            if (Coords.Equals(Target))
            {
                GiveInfo(this + " get target " + Coords, ConsoleColor.Blue);
                Target = null;
            }
            else
            {
                if (Coords.X == Target.X)
                {
                    if (Coords.Y > Target.Y)
                    {
                        Coords.Y--;
                    }
                    else
                    {
                        Coords.Y++;
                    }
                }
                else if (Coords.X > Target.X)
                {
                    Coords.X--;
                }
                else
                {
                    Coords.X++;
                }
                Travel();
            }
        }

        public void HideFromRain()
        {
            //GiveInfo(this + " open umbrella", ConsoleColor.Magenta);
            GoToHome = true;
            //new Thread(GoHome).Start();
        }

        void GoHome()
        {
            if (HaveHouse)
            {
                GiveInfo(this + " go home!", ConsoleColor.DarkMagenta);
                Target = HouseCoords;
                Travel();
            }
            else
            {
                GetHouse();
                GoHome();
            }
        }

        void GetHouse()
        {
            if (HaveHouse)
                UnregistrateFromCurrenHouse();

            foreach (House h in City.GetInstance().Houses)
                if (h.IsEmpty)
                {
                    h.LodgersList.Add(ID);
                    GiveInfo(this + " now have house at " + h.Coords, ConsoleColor.DarkGreen);
                    return;
                }

            BuildHouse();
            GetHouse();
        }

        void BuildHouse()
        {
            House houseToBild = House.GenerateHouse(Rand);
            Target = houseToBild.Coords;
            Travel();
            if (House.CanCreateInCity(houseToBild))
                PersonBuildHouse?.Invoke(houseToBild);
            else
                BuildHouse();
        }

        public void ChangeName(string name, string surname)
        {
            GiveInfo(this + " changed name to " + name + " " + surname, ConsoleColor.Gray);
            Name = name;
            Surname = surname;
        }

        void UnregistrateFromCurrenHouse()
        {
            if (HaveHouse)
            {
                // Go throw all for security from bugs
                foreach (House h in City.GetInstance().Houses)
                    h.LodgersList.RemoveAll(i => i == ID);
            }
        }

        void BornPerson()
        {
            Person p = new Person(ChooseName(), Surname, Rand);
            p.Coords = new Coordinates(Coords);
            PersonBorn?.Invoke(p, Reason.PersonBorned);
        }

        void GiveInfo(string str, ConsoleColor cc)
        {
            PersonInfo?.Invoke(str, cc);
        }

        public void Die()
        {
            UnregistrateFromCurrenHouse();
            City.GetInstance().RainEvent -= HideFromRain;
            PersonDie?.Invoke(this);
        }

        public string ChooseName()
        {
            return names[Rand.Next(names.Length)];
        }

        public override string ToString()
        {
            return FullName + " (ID: " + ID + ")";
        }

        public void Draw()
        {
            City.GetInstance().CityMap.SetPixel(Coords.X, Coords.Y, Color.Blue);
        }
    }
}
