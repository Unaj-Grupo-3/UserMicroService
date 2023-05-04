using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserCommands
    {
        Task<UserProfile> InsertUser(UserProfile user);

        Task<UserProfile> UpdateUser(Guid userId, UserUpdReq user);
    }
}
