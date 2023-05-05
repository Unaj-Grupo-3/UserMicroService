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

        public async Task<List<User>> GetAllUserByIds(List<int> userIds)
        {
            List<User> users = await _context.Users.Include(u => u.Images.OrderBy(x => x.Order))
                                                   .Include(u => u.Location)
                                                   .Include(u => u.Gender)
                                                   .Where(u => userIds.Contains(u.UserId)).ToListAsync();

            return users;
        }

        public async Task<List<User>> GetByFirstName(string? userFirstName)
        {
            if (userFirstName == null)
            {
                var users = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order))
                                                .Include(u => u.Location)
                                                .Include(u => u.Gender)
                                                .ToListAsync();

                return users;
            }
            else
            {
                var users = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order))
                                                .Include(u => u.Location)
                                                .Include(u => u.Gender)
                                                .Where(o => o.Name.ToLower().IndexOf(userFirstName.ToLower()) >= 0)
                                                .ToListAsync();

                return users;
            }
        }

        public async Task<User> GetUserByAuthId(Guid authId)
        {
            User user = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order))
                                            .Include(u => u.Location)
                                            .Include(u => u.Gender)
                                            .FirstOrDefaultAsync(x => x.AuthId == authId);

            return user;
        }

        public async Task<User> GetUserById(int userId)
        {
            User user = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order))
                                            .Include(u => u.Location)
                                            .Include(u => u.Gender)
                                            .FirstOrDefaultAsync(x => x.UserId == userId);

            return user;
        }

        public async Task<List<User>> GetUserByList()
        {
            var users = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order))
                                            .Include(u => u.Location)
                                            .Include(u => u.Gender)
                                            .ToListAsync();

            return users;
        }
    }
}
