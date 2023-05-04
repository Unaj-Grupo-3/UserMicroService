
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IServerImagesApiServices
    {
        Task<bool> UploadImage(IFormFile photo, Guid userId);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
