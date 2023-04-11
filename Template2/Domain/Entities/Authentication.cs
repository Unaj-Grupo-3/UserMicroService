﻿
namespace Domain.Entities
{
    public class Authentication
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public User User { get; set; }
    }
}
