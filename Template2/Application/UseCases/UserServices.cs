using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class UserServices : IUserServices
    {
        private readonly IUserCommands _commands;
        private readonly IAuthServices _authServices;

        public UserServices(IUserCommands commands, IAuthServices authServices)
        {
            _commands = commands;
            _authServices = authServices;
        }

        public async Task<UserResponse> AddUser(UserReq req)
        {
            await _authServices.CreateAuthentication(req.AuthReq);

            var auth = await _authServices.GetAuthentication(req.AuthReq);

            User user = new User
            {
                Name = req.Name,
                LastName = req.LastName,
                Birthday = req.Birthday,
                Description = req.Description,
                Gender = req.Gender,
                AuthId = auth.Id
            };

            await _commands.InsertUser(user);

            UserResponse resp = new UserResponse
            {
                Name = req.Name,
                LastName = req.LastName,
                Birthday = req.Birthday,
                Description = req.Description,
                Gender = req.Gender
            };

            return resp;
        }
    }
}
