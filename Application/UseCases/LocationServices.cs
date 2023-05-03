

using Application.Interfaces;

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
    }
}
