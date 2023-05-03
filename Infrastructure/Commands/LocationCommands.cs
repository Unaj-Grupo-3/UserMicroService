

using Application.Interfaces;
using Infrastructure.Persistance;

namespace Infrastructure.Commands
{
    public class LocationCommands : ILocationCommands
    {
        private readonly ExpresoDbContext _context;

        public LocationCommands(ExpresoDbContext context)
        {
            _context = context;
        }
    }
}
