using SQLiteMapaTeste.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

[assembly:Dependency(typeof(MapViewModel))]
namespace SQLiteMapaTeste.ViewModel
{
    public class MapViewModel : BaseViewModel
    {
        private Distance radius;

        public Distance Radius
        {
            get { return radius; }
            set => Set(ref radius, value);
        }

        private Position center;

        public Position Center
        {
            get { return center; }
            set => Set(ref center, value);
        }

        public MapViewModel()
        {
            Center= new Position(-23.4733, -46.6586);
            Radius = Distance.FromMeters(3000000000);
        }


    }
}
