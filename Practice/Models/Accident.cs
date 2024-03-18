namespace Practice.Models
{
    internal class Accident
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Longitude {  get; set; }
        public double Latitude { get; set; }

        public Accident(string name, string description, double longitude, double latitude)
        {
            Name = name;
            Description = description;
            Longitude = longitude;
            Latitude = latitude;
        }
        public Accident() { }
    }
}
