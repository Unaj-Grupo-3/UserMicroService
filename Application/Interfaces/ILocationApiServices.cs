using Application.Models;
using System.Text.Json;

namespace Application.Interfaces
{
    public interface ILocationApiServices
    {
        Task<LocationReq> GetLocation(string city);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
