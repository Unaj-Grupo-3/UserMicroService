
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IValidateImageServices
    {
        Task<bool> Validate(IFormFile photo, Guid userId);

        Task<bool> ValidateImages(IList<int> images, Guid userId);

        Dictionary<string, string> GetErrors();
    }
}
