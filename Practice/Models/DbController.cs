using Newtonsoft.Json.Linq;
using Practice.API;
using Practice;
using Practice.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;

internal static class DbController
{
    /// <summary>
    /// Додає інформацію про аварію до бази даних.
    /// </summary>
    /// <param name="accidentJson">Інформація про аварію у форматі JSON.</param>
    /// <param name="description">Додатковий опис аварії.</param>
    public static async Task AddAccident(JObject accidentJson, string description)
    {
        using (var database = new AccidentContext())
        {
            if (accidentJson.HasValues)
            {
                double lat = JsonDoubleConverter.Convert(accidentJson["lat"].ToString());
                double lon = JsonDoubleConverter.Convert(accidentJson["lon"].ToString());
                if (!(double.IsNaN(lon) || double.IsNaN(lat)))
                {
               
                    bool alreadyExist = await database.Accidents.AnyAsync(accident => accident.Longitude == lon && accident.Latitude == lat);
                    if (alreadyExist)
                    {
                        DialogManager.Notify("Ці дані вже були занесені в базу даних кимось до цього.", MessageBoxImage.Information, MessageBoxButton.OK);
                    }
                    else
                    {
                        string name = accidentJson["display_name"].ToString();

                        Accident accident = new Accident(name, description, lon, lat);
                        database.Accidents.Add(accident);
                        await database.SaveChangesAsync();
                        DialogManager.Notify("Дані було успішно занесено.", MessageBoxImage.Information, MessageBoxButton.OK);
                    }
                }
            }
        }
    }

    // Отримує всі аварії з бази даних.
    public static async Task<IEnumerable<Accident>> GetAllAccidents()
    {
        using (var database = new AccidentContext())
        {
            return await database.Accidents.ToListAsync();
        }
    }

    // Очищає всі дані про аварії з бази даних.
    public static async Task ClearAllAccidents()
    {
        if (!DialogManager.Notify("Ви впевнені, що хочете видалити всі дані з бази даних?", MessageBoxImage.Question, MessageBoxButton.YesNo)) return;

        using (var database = new AccidentContext())
        {
            database.Accidents.RemoveRange(database.Accidents);
            await database.SaveChangesAsync();
        }
    }
}
