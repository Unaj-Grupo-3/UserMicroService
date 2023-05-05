using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserCommands
    {
        Task<User> InsertUser(User user);

        Task<User> UpdateUser(int userId, UserUpdReq user, int locationId);
    }
}
