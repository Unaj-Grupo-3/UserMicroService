using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserQueries
    {
        Task<User> GetUserById(int userId);
        Task<List<User>> GetByFirstName(string? userFirstName);
        Task<User> GetUserByAuthId(Guid authId);
        Task<List<User>> GetUserByList();
        Task<List<User>> GetAllUserByIds(List<int> userIds);
    }
}
