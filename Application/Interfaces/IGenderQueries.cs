
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGenderQueries
    {
        Task<List<Gender>> GetGenders();
        Task<Gender> GetDescGenderById(int GenderId);
    }
}
