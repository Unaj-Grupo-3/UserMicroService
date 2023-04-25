

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

        public async Task<List<User>> GetByFirstName(string? userFirstName)
        {
            if (userFirstName == null)
            {
                var users = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order)).ToListAsync();

                return users;
            }
            else
            {
                var users = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order))
                                                 .Where(o => o.Name.ToLower().IndexOf(userFirstName.ToLower()) >= 0)
                                                 .ToListAsync();

                return users;
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            User user = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order)).FirstOrDefaultAsync(x => x.UserId == userId);

            return user;
        }
    }
}
