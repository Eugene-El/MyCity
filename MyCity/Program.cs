using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyCity
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            City city = City.GetInstance(100, 100, 10);
            Application map = new Application();
            map.Run(new MapWindow());


        }
    }
}
