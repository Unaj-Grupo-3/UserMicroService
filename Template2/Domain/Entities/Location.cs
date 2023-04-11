namespace Domain.Entities
{
    public class Location
    {
        public int LocationId { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public IList<User> Users { get; set; }
    }
}
