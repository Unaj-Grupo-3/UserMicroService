
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Queries
{
    public class LocationQueries: ILocationQueries
    {
        private readonly ExpresoDbContext _context;

        public LocationQueries(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task<Location> GetLocationByAddress(string city)
        {
            Location location = await _context.Locations.FirstOrDefaultAsync(x => x.City.ToLower() == city.ToLower());

            return location;
        }

        public async Task<Location> GetLocationByCoords(double latitude, double longitude)
        {
            Location location = await _context.Locations.FirstOrDefaultAsync(x => x.Latitude == latitude && x.Longitude == longitude);

            return location;
        }
    }
}
