using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public UserProfile User { get; set; }
    }
}
