

using Application.Interfaces;
using Domain.Entities;
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

        public async Task<Location> InsertLocation(Location location)
        {
            _context.Locations.Add(location);
            await  _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateLocation(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return location;
        }
    }
}
