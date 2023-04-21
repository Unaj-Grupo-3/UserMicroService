using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Application.UseCases
{
    public class UserServices : IUserServices
    {
        private readonly IUserCommands _commands;
        private readonly IAuthServices _authServices;
        private readonly IUserQueries _queries;

        public UserServices(IUserCommands commands,IUserQueries userQueries, IAuthServices authServices)
        {
            _commands = commands;
            _authServices = authServices;
            _queries = userQueries;
        }

        public async Task<UserResponse> AddUser(UserReq req)
        {

            await _authServices.CreateAuthentication(req.AuthReq);

            var auth = await _authServices.GetAuthentication(req.AuthReq);

            User user = new User
            {
                Name = req.Name,
                LastName = req.LastName,
                Birthday = req.Birthday.Value,
                Description = req.Description,
                Gender = req.Gender,
                AuthId = auth.Id
            };

            User create = await _commands.InsertUser(user);

            UserResponse resp = new UserResponse
            {
                UserId = create.UserId,
                Name = create.Name,
                LastName = create.LastName,
                Birthday = create.Birthday,
                Description = create.Description,
                Gender = create.Gender
            };

            return resp;
        }

        public async Task<UserResponse> UpdateUser(int userId, UserReq req)
        {
            var updated = await _commands.UpdateUser(userId, req);


            IList<string> images = new List<string>();

            if(updated.Images.Count != 0) 
            {
                foreach (var image in updated.Images)
                {
                    images.Add(image.Url);
                }
            }

            UserResponse response = new UserResponse
            {
                UserId = updated.UserId,
                Name = updated.Name,
                LastName = updated.LastName,
                Birthday = updated.Birthday,
                Description = updated.Description,
                Gender = updated.Gender,
                Images = images
            };

            return response;    
        }

        public async Task<UserResponse> GetUserById(int userId)
        {
            User user = await _queries.GetUserById(userId);

            if (user == null)
            {
                return null;
            }

            List<string> images = new List<string>();

            foreach(var image in user.Images)
            {
                images.Add(image.Url);
            }

            UserResponse response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                LastName = user.LastName,
                Birthday = user.Birthday,
                Gender = user.Gender,
                Description = user.Description,
                Images = images
            };

            return response;
        }
    }
}
