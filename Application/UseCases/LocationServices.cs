
using Application.Interfaces;
using Application.Models;

namespace Application.UseCases
{
    public class LocationServices : ILocationServices
    {
        private readonly ILocationCommands _commands;
        private readonly ILocationQueries _queries;

        public LocationServices(ILocationCommands commands, ILocationQueries queries)
        {
            _commands = commands;
            _queries = queries;
        }


        public Task<LocationResponse> AddLocation(LocationReq req)
        {
            throw new NotImplementedException();
        }

        public Task<LocationResponse> UpdateLocation(LocationReq req)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateLocation(LocationReq req, string address)
        {
            throw new NotImplementedException();
        }
    }
}
