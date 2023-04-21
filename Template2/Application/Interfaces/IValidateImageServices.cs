
namespace Application.Interfaces
{
    public interface IValidateImageServices
    {
        public Task<bool> ValidateUrl(string url);

        Dictionary<string, string> GetErrors();
    }
}
