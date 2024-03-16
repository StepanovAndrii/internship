using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.API
{
    internal class Roadway
    {
        private double _lat;
        private double _lon;
        private string _displayName;  

        public Roadway(double lat, double lon, string displayName)
        {
            _lat = lat;
            _lon = lon;
            _displayName = displayName;
        }
        public double Latitude
        {
            set => Latitude = value;
            get => Latitude;
        }
        public double Longitude
        {
            set => Longitude = value;
            get => Latitude;
        }
    }
}
