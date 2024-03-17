using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Models
{
    internal class Accident
    {
        public int Id { get; set; }
        public string FullAdress { get; set; }
        public string Description { get; set; }
        public double Longitude {  get; set; }
        public double Latitude { get; set; }
    }
}
