﻿using Application.Interfaces;
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

        public async Task<UserResponse> AddUser(UserReq req, int userId, int locationId)
        {

            User user = new User
            {
                UserId = userId,
                Name = req.Name,
                LastName = req.LastName,
                Birthday = req.Birthday.Value,
                Description = req.Description,
                GenderId = req.Gender.Value,
                LocationId = locationId,
            };

            User create = await _commands.InsertUser(user);

            UserResponse resp = MapperUserToUserResponse(create);
            
            return resp;
        }

        public async Task<UserResponse> UpdateUser(int userId, UserUpdReq req, int locationId)
        {
            var updated = await _commands.UpdateUser(userId, req, locationId);

            UserResponse response = MapperUserToUserResponse(updated);

            return response;    
        }

        public async Task<UserResponse> GetUserById(int userId)
        {
            User user = await _queries.GetUserById(userId);

            if (user == null)
            {
                return null;
            }

            UserResponse response = MapperUserToUserResponse(user);

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
                UserResponse response = MapperUserToUserResponse(user);

                userResponse.Add(response);
            }
            return userResponse;
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
                UserResponse response = MapperUserToUserResponse(user);

                userResponse.Add(response);
            }
            return userResponse;
        }

        public async Task<List<UserResponse>> GetAllUsersFullByIds(List<int> userIds)
        {
            List<UserResponse> listUserResponse = new List<UserResponse>();
            var listUser = await _queries.GetAllUserByIds(userIds);

            if (listUser.Any())
            {
                foreach (var user in listUser)
                {
                    UserResponse response = MapperUserToUserResponse(user);

                    listUserResponse.Add(response);
                }
                return listUserResponse;
            }
            else
            {
                return new List<UserResponse>();
            }
        }

        public async Task<List<UserViewResponse>> GetAllUsersByIds(List<int> userIds)
        {
            List<UserViewResponse> listUserResponse = new List<UserViewResponse>();
            var listUser = await _queries.GetAllUserByIds(userIds);

            if (listUser.Any())
            {
                foreach (var user in listUser)
                {
                    UserViewResponse response = MapperUserToUserViewResponse(user);

                    listUserResponse.Add(response);
                }
                return listUserResponse;
            }
            else
            {
                return new List<UserViewResponse>();
            }
        }

        public async Task<List<int>> GetUsersIdsByFilters(UserSuggestionRequest filters)
        {
            var usersByGenderId = await _queries.GetUsersByFilters(filters.GendersId);

            var usersByFilters = usersByGenderId.Where(x =>  Edad(x.Birthday) >= filters.MinEdad && Edad(x.Birthday) <= filters.MaxEdad && 
                                                             CalculateDistance(filters.Longitude,x.Location.Longitude,filters.Latitude,x.Location.Latitude) <= (filters.Distance + 1))
                                                        .ToList();

            var usersIdsByFilters = usersByFilters.Select(x => x.UserId).ToList();

            return usersIdsByFilters;
        }

        private UserResponse MapperUserToUserResponse(User user)
        {
            List<ImageResponse> imagesResponse = new List<ImageResponse>();

            if (user.Images != null)
            {
                foreach (var image in user.Images)
                {
                    ImageResponse imageResponse = new ImageResponse()
                    {
                        Id = image.ImageId,
                        Url = image.Url,
                    };

                    imagesResponse.Add(imageResponse);
                }
            }

            LocationResponse location = new LocationResponse();

            if (user.Location != null)
            {
                location.Id = user.Location.LocationId;
                location.Latitude = user.Location.Latitude;
                location.Longitude = user.Location.Longitude;
                location.Address = $"{user.Location.City}, {user.Location.Province}, {user.Location.Country}";
            }

            UserResponse response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                LastName = user.LastName,
                Birthday = user.Birthday,
                Gender = new GenderResponse()
                {
                    GenderId = user.GenderId,
                    Description = user.Gender.Description,
                },
                Description = user.Description,
                Images = imagesResponse,
                Location = new LocationResponse()
                {
                    Id = location.Id,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Address = location.Address
                }
            };

            return response;
        }

        private UserViewResponse MapperUserToUserViewResponse(User user)
        {
            string imageResponse = "";
          
            if (user.Images.Any())
            {
                var image = user.Images.OrderBy(i => i.Order).FirstOrDefault();
                imageResponse = image.Url;
            }    

            UserViewResponse response = new UserViewResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                LastName = user.LastName,
                Gender = user.Gender.Description,
                Image = imageResponse
            };

            return response;
        }


        private int CalculateDistance(double longitude1, double longitude2, double latitude1, double latitude2)
        {
            double theta = longitude1 - longitude2;

            double distance = 60 * 1.1515 * (180 / Math.PI) * Math.Acos(
                 Math.Sin(latitude1 * (Math.PI / 180)) * Math.Sin(latitude2 * (Math.PI / 180)) +
                 Math.Cos(latitude1 * (Math.PI / 180)) * Math.Cos(latitude2 * (Math.PI / 180)) * Math.Cos(theta * (Math.PI / 180))
                );

            return Convert.ToInt32(Math.Round(distance * 1.609344, 2));
        }


        private int Edad(DateTime date)
        {
            return DateTime.Today.AddTicks(-date.Ticks).Year - 1; ;
        }
    }
    
}
