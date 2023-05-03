namespace Domain.Entities
{
    public class Location
    {
        public int LocationId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public IList<User> Users { get; set; }
    }
}
