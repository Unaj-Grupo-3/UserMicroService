using Application.Models;

namespace Application.Interfaces
{
    public interface IGenderServices
    {
        Task<List<GenderResponse>> GetGenders();
        Task<GenderResponse> GetDescGenderById(int GenderId);
    }
}
