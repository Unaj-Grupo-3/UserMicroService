

using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILocationCommands
    {
        Task<Location> InsertLocation(Location location);
        Task<Location> UpdateLocation(Location location);
    }
}
