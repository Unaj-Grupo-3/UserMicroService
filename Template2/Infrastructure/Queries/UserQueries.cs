

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly ExpresoDbContext _context;

        public UserQueries(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserById(int userId)
        {
            User user = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order)).FirstOrDefaultAsync(x => x.UserId == userId);

            return user;
        }
    }
}
