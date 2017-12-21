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
        private int currentPersonID;

        public MapWindow()
        {
            InitializeComponent();

            PeopleGrid.ItemsSource = City.GetInstance().PeopleCollection;
            
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
                        
                        PeopleGrid.ItemsSource = null;
                        PeopleGrid.ItemsSource = City.GetInstance().PeopleCollection;

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ClearFields()
        {
            TBname.Text = TBsurname.Text = "";
        }

        private void BtnAddNewPerson_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            GBaddEdit.Visibility = Visibility.Visible;
            BtnAddNewPerson.Visibility = Visibility.Hidden;
            currentPersonID = 0;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            GBaddEdit.Visibility = Visibility.Hidden;
            BtnAddNewPerson.Visibility = Visibility.Visible;
            currentPersonID = 0;
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (currentPersonID == 0)
            {
                Person person = new Person(TBname.Text, TBsurname.Text, new Random());
                person.Coords = Coordinates.GetCoordsOnCityBorder(new Random());
                City.GetInstance().AddPerson(person, Reason.PersonArived);
            }
            else
            {
                Person person = City.GetInstance().People.Find(p => p.ID == currentPersonID);
                if (person != null)
                    person.ChangeName(TBname.Text, TBsurname.Text);

            }

            ClearFields();
            GBaddEdit.Visibility = Visibility.Hidden;
            BtnAddNewPerson.Visibility = Visibility.Visible;
            currentPersonID = 0;
        }

        private void PeopleGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Person person = (Person)PeopleGrid.SelectedItem;
            currentPersonID = person.ID;

            ClearFields();
            GBaddEdit.Visibility = Visibility.Visible;
            BtnAddNewPerson.Visibility = Visibility.Hidden;

            TBname.Text = person.Name;
            TBsurname.Text = person.Surname;
        }
    }
}
