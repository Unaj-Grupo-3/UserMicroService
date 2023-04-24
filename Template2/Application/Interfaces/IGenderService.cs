using Application.Models;

namespace Application.Interfaces
{
    public interface IGenderService
    {
        Task<List<GenderResponse>> GetGenders();
        Task<GenderResponse> GetDescGenderById(int GenderId);
    }
}
