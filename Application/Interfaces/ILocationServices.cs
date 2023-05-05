using Application.Models;

namespace Application.Interfaces
{
    public interface ILocationServices
    {
        Task<bool> ValidateLocation(LocationReq req, string address);
        Task<LocationResponse> AddLocation(LocationReq req);
        Task<LocationResponse> UpdateLocation(LocationReq req);
    }
}
