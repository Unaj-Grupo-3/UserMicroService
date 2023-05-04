using Application.Models;

namespace Application.Interfaces
{
    public interface IUserServices
    {
        Task<UserResponse> AddUser(UserReq req, Guid authId);

        Task<UserResponse> UpdateUser(Guid userId, UserUpdReq req);

        Task<UserResponse> GetUserById(Guid userId);

        Task<List<UserResponse>> GetByFirstName(string? userFirstName);

        Task<UserResponse> GetUserByAuthId(Guid authId);

        Task<List<UserResponse>> GetUserByList();

        Task<List<UserResponse>> GetAllUserByIds(List<Guid> userIds);
    }
}
