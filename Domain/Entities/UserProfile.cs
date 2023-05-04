using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class UserProfile
    {
        public Guid AuthId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Description { get; set; }
        public int? LocationId { get; set; }

        public int GenderId { get; set; }
        public Location? Location { get; set; }
        public IList<Image>? Images { get; set; }
        public Gender Gender { get; set; }
        
        public int UserStatus { get; set; } // 1: Datos Principales | 2: Preferencias o Fotos | 3: Completo 

    }
}
