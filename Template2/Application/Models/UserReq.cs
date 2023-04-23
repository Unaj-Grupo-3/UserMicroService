
namespace Application.Models
{
    public class UserReq
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Description { get; set; }
        public string? Gender { get; set; }
        public AuthReq? AuthReq { get; set; }
        public IList<string>? Images { get; set; }
    }
}
