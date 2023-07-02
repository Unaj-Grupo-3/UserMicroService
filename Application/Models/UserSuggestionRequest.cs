

namespace Application.Models
{
    public class UserSuggestionRequest
    {
        public List<int>? GendersId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Distance { get; set; }
        public int MinEdad { get; set; }
        public int MaxEdad { get; set;}
    }
}
