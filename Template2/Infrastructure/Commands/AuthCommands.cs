using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands
{
    public class AuthCommands : IAuthCommands
    {
        private readonly ExpresoDbContext _context;

        public AuthCommands(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task InsertAuthentication(Authentication auth)
        {
            _context.Add(auth);

            await _context.SaveChangesAsync();
        }

        
    }
}
