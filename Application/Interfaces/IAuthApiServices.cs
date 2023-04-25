
using Application.Models;
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IAuthApiServices
    {
        Task<bool> CreateAuth(AuthReq req);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
