using System.Text.Json;

namespace Application.Interfaces
{
    public interface ILocationServices
    {
        Task<string> GetLocation(string req);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
