
using Application.Interfaces;
using Application.Models;
using Domain.Entities;

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


        public async Task<LocationResponse> AddLocation(LocationReq req)
        {
            Location location = LocationReqToLocation(req);

            Location create = await _commands.InsertLocation(location);

            return LocationToLocationResponse(create);

        }

        public async Task<LocationResponse> GetLocationByCity(string city)
        {
            Location locationByCity = await _queries.GetLocationByAddress(city);

            if (locationByCity == null)
            {
                return null;
            }

            return LocationToLocationResponse(locationByCity);
        }

        public async Task<LocationResponse> GetLocationByCoords(double latitude, double longitude)
        {
            Location locationByCoords = await _queries.GetLocationByCoords(latitude, longitude);

            if (locationByCoords == null)
            {
                return null;
            }

            return LocationToLocationResponse(locationByCoords);
        }

        public async Task<LocationResponse> UpdateLocation(LocationReq req)
        {
            Location location = LocationReqToLocation(req);

            Location updated = await _commands.UpdateLocation(location);

            return LocationToLocationResponse(updated);
        }

        private Location LocationReqToLocation(LocationReq req)
        {

            return new Location
            {
                Latitude = req.Latitude.Value,
                Longitude = req.Longitude.Value,
                City = req.City,
                Province = req.Province,
                Country = req.Country,
            };
        }

        private LocationResponse LocationToLocationResponse(Location location)
        {
            return new LocationResponse
            {
                Id = location.LocationId,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Address = $"{location.City}, {location.Province}, {location.Country}"
            };
        }
    }
}
