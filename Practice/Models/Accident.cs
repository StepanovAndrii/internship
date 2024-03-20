namespace Practice.Models
{
    /// <summary>
    /// Клас, що представляє інформацію про подію.
    /// </summary>
    internal class Accident
    {
        /// <summary>
        /// Унікальний ідентифікатор події.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Назва події.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Опис події.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Довгота події.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Широта події.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Ініціалізує новий екземпляр класу Accident з вказаними даними.
        /// </summary>
        /// <param name="name">Назва події.</param>
        /// <param name="description">Опис події.</param>
        /// <param name="longitude">Довгота події.</param>
        /// <param name="latitude">Широта події.</param>
        public Accident(string name, string description, double longitude, double latitude)
        {
            Name = name;
            Description = description;
            Longitude = longitude;
            Latitude = latitude;
        }

        /// <summary>
        /// Ініціалізує новий екземпляр класу Accident без параметрів.
        /// </summary>
        public Accident() { }
    }
}
