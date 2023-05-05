

using Application.Models;

namespace Application.Interfaces
{
    public interface IValidateLocationServices
    {
        bool ValidateLocation(LocationReq req);

        Dictionary<string, string> GetErrors();

        bool GenerateLocation();

    }
}
