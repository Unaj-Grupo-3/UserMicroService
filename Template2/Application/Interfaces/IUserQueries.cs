
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserQueries
    {
        Task<User> GetUserById(int userId);
    }
}
