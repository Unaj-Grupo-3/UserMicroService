using Application.Models;

namespace Application.Interfaces
{
    public interface IUserServices
    {
        Task<UserResponse> AddUser(UserReq req, Guid authId);

        Task<UserResponse> UpdateUser(int userId, UserUpdReq req);

        Task<UserResponse> GetUserById(int userId);

        Task<List<UserResponse>> GetByFirstName(string? userFirstName);

        Task<UserResponse> GetUserByAuthId(Guid authId); 
    }
}
