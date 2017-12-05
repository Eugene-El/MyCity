using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCity
{
    delegate void PersonBornDelegate(Person person, Reason reason);
    delegate void PersonInfoDelegate(string str);
    delegate void PersonDieDelegate(Person person);

    class Person
    {
        private static int currentID = 1;
        private static string[] names = { "John", "Jack", "Luis", "Eugene", "Kirill", "Svetlana", "Ann", "Boris", "Denis", "Poll", "Serge", "Mark", "Ricardo", "Tina", "Leo", "Anton" };
        private static string[] surnames = { "Smith", "Holand", "Rufalo", "Alvarez", "Mango", "Cant", "Huff" };
        private Random Rand;
        public int ID { get; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName { get { return Name + " " + Surname; } }
        public DateTime Birthday { get; }
        public Coordinates Coords { get; set; }

        // Targeting
        private Coordinates target;
        public Coordinates Target {
            get { return target; }
            set { target = value; HaveTarget = true; }
        }
        public bool HaveTarget { get; private set; }
        //

        public event PersonBornDelegate PersonBorn;
        public event PersonInfoDelegate PersonInfo;
        public event PersonDieDelegate PersonDie;


        public Person(string name, string surname, Random rand)
        {
            ID = currentID++;
            Name = name;
            Surname = surname;
            Rand = rand;
            Birthday = DateTime.Now;
            Coords = new Coordinates();

            Thread LiveThread = new Thread(Live);
            LiveThread.Start();
        }

        public Person(Random rand)
        {
            ID = currentID++;
            Rand = rand;
            Birthday = DateTime.Now;
            Name = ChooseName();
            Surname = surnames[Rand.Next(surnames.Length)];
            Coords = new Coordinates();

            Thread LiveThread = new Thread(Live);
            LiveThread.Start();

        }

        void Live()
        {
            Thread.Sleep(Rand.Next(10000, 60000)); // From 10 to 60 seconds
            int choose = Rand.Next(100);
            if (choose < 30)
            {
                BornPerson();
            }
            else if (choose < 60)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Target = new Coordinates(Rand);
                GiveInfo(this + " go to " + Target);
                Travel();
            }
            else if (choose > 95)
            {
                Die();
                return;
            }
            
            Live();
        }

        void Travel()
        {
            Thread.Sleep(1500); // 1.5 seconds (standart speed for all)

            Console.ForegroundColor = ConsoleColor.Magenta;
            //GiveInfo(this + " now at" + Coords);
            if (Coords.Equals(Target))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                GiveInfo(this + " get target " + Coords);
                HaveTarget = false;
                Live();
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


        public void BornPerson()
        {
            PersonBorn?.Invoke(new Person(ChooseName(), Surname, Rand), Reason.PersonBorned);
        }

        public void GiveInfo(string str)
        {
            PersonInfo?.Invoke(str);
        }

        public void Die()
        {
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
    }
}
