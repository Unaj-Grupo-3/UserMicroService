
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IValidateImageServices
    {
        Task<bool> Validate(IFormFile photo, int userId);

        Task<bool> ValidateImages(IList<int> images, int userId);

        Dictionary<string, string> GetErrors();
    }
}
