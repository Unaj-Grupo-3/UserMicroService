using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserCommands
    {
        Task<User> InsertUser(User user);
    }
}
