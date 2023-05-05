using Application.Models;

namespace Application.Interfaces
{
    public interface ILocationServices
    {
        Task<LocationResponse> AddLocation(LocationReq req);
        Task<LocationResponse> UpdateLocation(LocationReq req);
        Task<LocationResponse> GetLocationByCity(string city);
        Task<LocationResponse> GetLocationByCoords(double latitude, double longitude);
    }
}
