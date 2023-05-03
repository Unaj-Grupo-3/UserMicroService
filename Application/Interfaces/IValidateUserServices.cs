using Application.Models;

namespace Application.Interfaces
{
    public interface IValidateUserServices
    {
        Task<bool> ValidateLenght(string verify, string tag, int maxLenght);
        Task<bool> ValidateCharacters(string verify, string tag);
        Task<bool> ValidateAge(DateTime? verify, string tag);
        Task<IDictionary<bool, IDictionary<string, string>>> Validate(UserReq userReq, bool allowNull);
        Task<IDictionary<bool, IDictionary<string, string>>> Validate(UserUpdReq userUpdReq, bool allowNull);
    }
}
