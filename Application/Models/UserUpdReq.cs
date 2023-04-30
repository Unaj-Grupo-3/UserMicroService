
namespace Application.Models
{
    public class UserUpdReq
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Description { get; set; }
        public int? Gender { get; set; }
    }
}