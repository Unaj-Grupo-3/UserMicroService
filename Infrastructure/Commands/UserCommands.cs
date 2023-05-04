using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands
{
    public class UserCommands : IUserCommands
    {
        private readonly ExpresoDbContext _context;

        public UserCommands(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> InsertUser(UserProfile user)
        {
            _context.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<UserProfile> UpdateUser(Guid userId, UserUpdReq user)
        {
            UserProfile updated = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            updated.Name = user.Name != null ? user.Name : updated.Name;
            updated.LastName = user.LastName != null ? user.LastName : updated.LastName;
            updated.Birthday = user.Birthday != null ? user.Birthday.Value : updated.Birthday;
            updated.GenderId = user.Gender != null ? user.Gender.Value : updated.GenderId;
            updated.Description = user.Description != null ? user.Description : updated.Description;

            await _context.SaveChangesAsync();

            return updated;
        }
    }
}
