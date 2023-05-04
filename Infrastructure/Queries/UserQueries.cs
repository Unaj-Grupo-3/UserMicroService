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

        public async Task<List<UserProfile>> GetAllUserByIds(List<Guid> userIds)
        {
            List<UserProfile> users = await _context.Users.Include(u => u.Images.OrderBy(x => x.Order))
                                                         .Where(u => userIds.Contains(u.UserId)).ToListAsync();

            return users;
        }

        public async Task<List<UserProfile>> GetByFirstName(string? userFirstName)
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

        public async Task<UserProfile> GetUserByAuthId(Guid authId)
        {
            UserProfile user = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order)).FirstOrDefaultAsync(x => x.AuthId == authId);

            return user;
        }

        public async Task<UserProfile> GetUserById(Guid userId)
        {
            UserProfile user = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order)).FirstOrDefaultAsync(x => x.UserId == userId);

            return user;
        }

        public async Task<List<UserProfile>> GetUserByList()
        {
            var users = await _context.Users.Include(x => x.Images.OrderBy(x => x.Order))
                                                 .ToListAsync();

            return users;
        }
    }
}
