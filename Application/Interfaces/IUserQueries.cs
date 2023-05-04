using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserQueries
    {
        Task<UserProfile> GetUserById(Guid userId);
        Task<List<UserProfile>> GetByFirstName(string? userFirstName);
        Task<UserProfile> GetUserByAuthId(Guid authId);
        Task<List<UserProfile>> GetUserByList();
        Task<List<UserProfile>> GetAllUserByIds(List<Guid> userIds);
    }
}
