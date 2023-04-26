using Domain.Entities;

namespace Application.Models
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Description { get; set; }
        public int Gender { get; set; }
        public Location? Location { get; set; }
        public IList<string> Images { get; set; }
    }
}
