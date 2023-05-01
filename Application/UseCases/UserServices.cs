using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Application.UseCases
{
    public class UserServices : IUserServices
    {
        private readonly IUserCommands _commands;
        private readonly IUserQueries _queries;

        public UserServices(IUserCommands commands,IUserQueries userQueries)
        {
            _commands = commands;
            _queries = userQueries;
        }

        public async Task<UserResponse> AddUser(UserReq req, Guid authId)
        {

            User user = new User
            {
                Name = req.Name,
                LastName = req.LastName,
                Birthday = req.Birthday.Value,
                Description = req.Description,
                GenderId = req.Gender.Value,
                AuthId = authId
            };

            User create = await _commands.InsertUser(user);

            UserResponse resp = new UserResponse
            {
                UserId = create.UserId,
                Name = create.Name,
                LastName = create.LastName,
                Birthday = create.Birthday,
                Description = create.Description,
                Gender = create.GenderId,
                Images = new List<ImageResponse>()
            };

            return resp;
        }

        public async Task<UserResponse> UpdateUser(int userId, UserUpdReq req)
        {
            var updated = await _commands.UpdateUser(userId, req);

            List<ImageResponse> imagesResponse = new List<ImageResponse>();

            foreach (var image in updated.Images)
            {
                ImageResponse imageResponse = new ImageResponse()
                {
                    Id = image.ImageId,
                    Url = image.Url,
                };

                imagesResponse.Add(imageResponse);
            }

            UserResponse response = new UserResponse
            {
                UserId = updated.UserId,
                Name = updated.Name,
                LastName = updated.LastName,
                Birthday = updated.Birthday,
                Description = updated.Description,
                Gender = updated.GenderId,
                Images = imagesResponse
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

            List<ImageResponse> imagesResponse = new List<ImageResponse>();

            foreach(var image in user.Images)
            {
                ImageResponse imageResponse = new ImageResponse()
                {
                    Id = image.ImageId,
                    Url = image.Url,
                };

                imagesResponse.Add(imageResponse);
            }

            UserResponse response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                LastName = user.LastName,
                Birthday = user.Birthday,
                Gender = user.GenderId,
                Description = user.Description,
                Images = imagesResponse
            };

            return response;
        }

        public async Task<List<UserResponse>> GetByFirstName(string? userFirstName)
        {
            var users = await _queries.GetByFirstName(userFirstName);

            List<UserResponse> userResponse = new List<UserResponse>();

            if (users.Count == 0)
            {
                return new List<UserResponse>();
            }
            foreach (var user in users)
            {
                List<ImageResponse> imagesResponse = new List<ImageResponse>();

                foreach (var image in user.Images)
                {
                    ImageResponse imageResponse = new ImageResponse()
                    {
                        Id = image.ImageId,
                        Url = image.Url,
                    };

                    imagesResponse.Add(imageResponse);
                }

                UserResponse response = new UserResponse
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    LastName = user.LastName,
                    Birthday = user.Birthday,
                    Gender = user.GenderId,
                    Description = user.Description,
                    Images = imagesResponse
                };

                userResponse.Add(response);
            }
            return userResponse;
        }

        public async Task<UserResponse> GetUserByAuthId(Guid authId)
        {
            User user = await _queries.GetUserByAuthId(authId);

            if (user == null)
            {
                return null;
            }

            List<ImageResponse> imagesResponse = new List<ImageResponse>();

            foreach (var image in user.Images)
            {
                ImageResponse imageResponse = new ImageResponse()
                {
                    Id = image.ImageId,
                    Url = image.Url,
                };

                imagesResponse.Add(imageResponse);
            }

            UserResponse response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                LastName = user.LastName,
                Birthday = user.Birthday,
                Gender = user.GenderId,
                Description = user.Description,
                Images = imagesResponse
            };

            return response;
        }

        public async Task<List<UserResponse>> GetUserByList()
        {
            var users = await _queries.GetUserByList();

            List<UserResponse> userResponse = new List<UserResponse>();

            if (users.Count == 0)
            {
                return new List<UserResponse>();
            }
            foreach (var user in users)
            {
                List<ImageResponse> imagesResponse = new List<ImageResponse>();

                foreach (var image in user.Images)
                {
                    ImageResponse imageResponse = new ImageResponse()
                    {
                        Id = image.ImageId,
                        Url = image.Url,
                    };

                    imagesResponse.Add(imageResponse);
                }

                UserResponse response = new UserResponse
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    LastName = user.LastName,
                    Birthday = user.Birthday,
                    Gender = user.GenderId,
                    Description = user.Description,
                    Images = imagesResponse
                };
                userResponse.Add(response);
            }
            return userResponse;
        }

        public async Task<List<UserResponse>> GetAllUserByIds(List<int> userIds)
        {
            List<UserResponse> listUserResponse = new List<UserResponse>();
            var listUser = await _queries.GetAllUserByIds(userIds);

            if (listUser.Any())
            {
                foreach (var user in listUser)
                {
                    List<ImageResponse> imagesResponse = new List<ImageResponse>();

                    foreach (var image in user.Images)
                    {
                        ImageResponse imageResponse = new ImageResponse()
                        {
                            Id = image.ImageId,
                            Url = image.Url,
                        };

                        imagesResponse.Add(imageResponse);
                    }

                    UserResponse response = new UserResponse
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        LastName = user.LastName,
                        Birthday = user.Birthday,
                        Gender = user.GenderId,
                        Description = user.Description,
                        Images = imagesResponse
                    };

                    listUserResponse.Add(response);
                }
                return listUserResponse;
            }
            else
            {
                return new List<UserResponse>();
            }
        }
    }
    
}
