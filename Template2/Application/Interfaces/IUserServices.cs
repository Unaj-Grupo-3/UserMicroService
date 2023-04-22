using Application.Models;

namespace Application.Interfaces
{
    public interface IUserServices
    {
        Task<UserResponse> AddUser(UserReq req, Guid authId);

        Task<UserResponse> UpdateUser(int userId,UserReq req); 

        Task<UserResponse> GetUserById(int userId);
    }
}
