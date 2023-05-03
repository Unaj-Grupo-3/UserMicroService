
using Application.Interfaces;
using Infrastructure.Persistance;

namespace Infrastructure.Queries
{
    public class LocationQueries: ILocationQueries
    {
        private readonly ExpresoDbContext _context;

        public LocationQueries(ExpresoDbContext context)
        {
            _context = context;
        }
    }
}
