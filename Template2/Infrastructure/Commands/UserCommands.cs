using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands
{
    public class UserCommands : IUserCommands
    {
        private readonly ExpresoDbContext _context;

        public UserCommands(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task InsertUser(User user)
        {
            _context.Add(user);

            await _context.SaveChangesAsync();
        }
    }
}
