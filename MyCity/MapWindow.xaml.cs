using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyCity
{
    /// <summary>
    /// Логика взаимодействия для MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow()
        {
            InitializeComponent();

            Thread t = new Thread(Update);
            t.Start();
        }


        void Update()
        {
            Thread.Sleep(500);

            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {

                    if (City.GetInstance().CityMap != null)
                    {
                        City.GetInstance().CityMap.Draw();
                        ImgMap.Source = BitmapToImageSource((Bitmap)City.GetInstance().CityMap.Map.Clone());

                        LblPopulation.Content = "Population: " + City.GetInstance().People.Count;
                        LblHouses.Content = "Houses count: " + City.GetInstance().Houses.Count;
                    }
                }));
            }
            catch (Exception) { }

            Update();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
