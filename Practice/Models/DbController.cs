using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using Practice.API;

namespace Practice.Models
{
    internal static class DbController
    {
        /// <summary>
        /// Додає дані про аварію в базу даних.
        /// </summary>
        /// <param name="accidentJson">Об'єкт у форматі JObject який має в собі дані про аварію.</param>
        /// <param name="description">Опис про аварію.</param>
        public static async Task AddAccident(JObject accidentJson, string description)
        {
            using (AccidentContext database = new AccidentContext())
            {
                try
                {
                    if(accidentJson.HasValues) // Перевірка на те, чи пустий JObject ({})
                    {
                        double lon = JsonDoubleConverter.Convert(accidentJson["lon"].ToString()); // Отримання значення ключа lon(довгота) та конвертація в формат double під C#
                        double lat = JsonDoubleConverter.Convert(accidentJson["lat"].ToString()); // Отримання значення ключа lat(широта) та конвертація в формат double під C#
                        string name = accidentJson["display_name"].ToString(); // Отримання значення ключа display_name(адреса) і збереження у форматі строки

                        bool exists = await database.Accidents.AnyAsync(accident => accident.Longitude == lon && accident.Latitude == lat); // Асинхронний пошук по базі даних за допомогою linq по координатам (чи є вже така адреса)

                        if (!(double.IsNaN(lon) || double.IsNaN(lat))) // Методі JsonDoubleConverter.Convert повертає значення NaN якщо не вийшло конвертувати. Йде перевірка на ці значення
                        {
                            if (exists)
                            {
                                DialogManager.DisplayInfo("Інформація про цю аварію вже присутня в базі даних."); // Виведення інформації про те, що дані цієї аварії вже присутні в базі даних
                            }
                            else
                            {
                                Accident finalData = new Accident(name, description, lon, lat);
                                database.Accidents.Add(finalData);
                                await database.SaveChangesAsync();
                                DialogManager.DisplayInfo("Аварію було додано.");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    DialogManager.ExitWithError($"Під час виконання сталася помилка."); // Виведення помилки про те, що під час виконання сталась якась невідома помилка і вихід з додатку
                }
            }
        }

        /// <summary>
        /// Повертає всі аварії що записані в базі даних.
        /// </summary>
        /// <returns>Повертає перелік всіх записів аварій у форматі IEnumerable.</returns>
        public static async Task<IEnumerable<Accident>> GetAllAccidents()
        {
            using (AccidentContext database = new AccidentContext()) // Використання конструкції using для автоматичного керування ресурсами бази даних. Після завершення виконання блоку using, об'єкт database та всі його ресурси будуть видалені з пам'яті
            {
                try
                {
                    return await database.Accidents.ToListAsync(); // Асинхронно конвертує базу даних в List та повертає з методу
                }
                catch(Exception)
                {
                    DialogManager.ExitWithError($"Під час виконання сталася помилка.");
                    return Enumerable.Empty<Accident>(); // При помилці повертає пустий IEnumerable.
                }
            }
        }

        /// <summary>
        /// Видаляє всі дані збережені в базі даних на даний момент.
        /// </summary>
        public static async Task ClearAllAccidents()
        {
            using (AccidentContext database = new AccidentContext()) // Використання конструкції using для автоматичного керування ресурсами бази даних. Після завершення виконання блоку using, об'єкт database та всі його ресурси будуть видалені з пам'яті
            {
                bool agreed = DialogManager.DisplayQuestion("Ви впевнені, що хочете видалити всі дані з бази даних?"); // Виводить питальне діалогове вікно і записує булевий результат
                if (agreed)
                {
                    try
                    {
                        database.Accidents.RemoveRange(database.Accidents); // Видаляє все що є в базі даних
                        await database.SaveChangesAsync(); // Зберігає зміни в базі даних
                        DialogManager.DisplayInfo("Дані успішно видаленно."); // Виводить діалогове інформаційне вікно про успішність операції
                    }
                    catch (Exception)
                    {
                        DialogManager.ExitWithError($"Під час виконання сталася помилка.");
                    }
                }
            }
        }
    }
}
