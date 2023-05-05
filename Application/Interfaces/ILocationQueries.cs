using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILocationQueries
    {
        Task<Location> GetLocationByCoords(double latitude, double longitude);

        Task<Location> GetLocationByAddress(string city);
    }
}
