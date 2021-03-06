﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCity
{
    class CityMap
    {
        const int SCALE = 5;

        private Bitmap sunnyMap;
        private Bitmap rainyMap;
        private Bitmap Background;


        public Bitmap Map { get; private set; }

        int Width { get { return City.GetInstance().Width; } }
        int Height { get { return City.GetInstance().Height; } }
        int RealWidth { get { return Width * SCALE; } }
        int RealHeight { get { return Height * SCALE; } }

        public void Draw()
        {
            try
            {
                ClearMap();
                foreach (House h in City.GetInstance().Houses)
                    h.Draw();
                foreach (Person p in City.GetInstance().People)
                    p.Draw();
            }
            catch (Exception) { }
        }

        public CityMap()
        {
            rainyMap = new Bitmap(RealWidth, RealHeight);
            sunnyMap = new Bitmap(RealWidth, RealHeight);

            FillBackground(rainyMap, Color.ForestGreen);
            FillBackground(sunnyMap, Color.LimeGreen);

            MakeSunnyMap();

            MakeSunnyMap();
            ClearMap();
        }

        public void FillBackground(Bitmap bitmap, Color color)
        {
            try
            {
                for (int j = 0; j < RealHeight; j++)
                    for (int i = 0; i < RealWidth; i++)
                        bitmap.SetPixel(j, i, color);
            }
            catch (Exception) { FillBackground(bitmap, color); }
        }

        private void ClearMap()
        {
            Map = (Bitmap)Background.Clone();
        }

        public void MakeSunnyMap()
        {
            Background = (Bitmap)sunnyMap.Clone();
        }

        public void MakeRainnyMap()
        {
            Background = (Bitmap)rainyMap.Clone();
        }


        public void SetPixel(int x, int y, Color color)
        {
            for (int j = 0; j < SCALE; j++)
                for (int i = 0; i < SCALE; i++)
                    Map.SetPixel(x*SCALE+i, y*SCALE+j, color);
        }


        public void Save()
        {
            Map.Save("MyCity"+DateTime.Now.ToShortDateString()+'.'+DateTime.Now.ToLongTimeString().Replace(':','.')+".png");
        }
    }
}
