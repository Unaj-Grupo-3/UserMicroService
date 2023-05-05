using System.Text.Json;

namespace Application.Interfaces
{
    public interface ILocationApiServices
    {
        Task<string> GetLocation(string req);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
