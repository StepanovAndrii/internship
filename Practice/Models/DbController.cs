using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows;

namespace Practice.Models
{
    internal static class DbController
    {
        public static void AddAccident(string fullAddress, string description, double lon, double lan)
        {
            using (AccidentContext db = new AccidentContext()) 
            {
                Accident accident = new Accident {FullAdress = fullAddress, Description = description, Longitude = lon, Latitude = lan };
                db.Accidents.Add(accident);
                db.SaveChanges();
            }
        }
        public static void AddAccident(JObject data)
        {
            using (AccidentContext db = new AccidentContext())
            {
                Accident accident = new Accident { FullAdress = data["name"].ToString(), Description = "опис", Longitude = double.Parse(data["lon"].ToString().Trim().Replace('.', ',')), Latitude = double.Parse(data["lat"].ToString().Trim().Replace('.', ',')) };
                db.Accidents.Add(accident);
                db.SaveChanges();
                foreach (var item in db.Accidents)
                {
                    MessageBox.Show(item.FullAdress, "lsrlgsdr", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        public static void DeleteAccident(int id)
        {
            using (AccidentContext db = new AccidentContext())
            {
                foreach (Accident accident in db.Accidents)
                {
                    if(accident.Id == id)
                    {
                        db.Accidents.Remove(accident);
                    }
                }
                db.SaveChanges();
            }
        }
        public static IEnumerable<Accident> GetAllAccidents()
        {
            using (AccidentContext db = new AccidentContext())
            {
                return db.Accidents;
            }
        }
    }
}
