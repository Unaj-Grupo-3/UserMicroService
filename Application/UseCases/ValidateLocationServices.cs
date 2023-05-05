

using Application.Interfaces;
using Application.Models;

namespace Application.UseCases
{
    public class ValidateLocationServices : IValidateLocationServices
    {
        private Dictionary<string, string> errors;
        private bool GenerateLocationWithApi;

        public ValidateLocationServices()
        {
            errors = new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetErrors()
        {

            return errors;
        }

        public bool ValidateLocation(LocationReq req)
        {
            bool latIsValid = true;
            bool lgnIsValid = true;
            bool countryIsValid = true;
            bool provinceIsValid = true;
            bool cityIsValid = true;

            if (req.Latitude == null)
            {
                latIsValid = false;
            }

            if (req.Longitude == null)
            {
                lgnIsValid = false;
            }

            if (req.Country == null || req.Country.Trim() == "")
            {
                countryIsValid = false;
            }

            if (req.Province == null || req.Province.Trim() == "")
            {
                provinceIsValid = false;
            }

            if (req.City.Trim() == "")
            {
                cityIsValid = false;
            }

            bool otherIsValid = latIsValid && lgnIsValid && countryIsValid && provinceIsValid;

            if (!cityIsValid)
            {
                errors.Add("Location", "Ingrese una ubicacion valida");
                return false;
            }

            if (cityIsValid && !otherIsValid) 
            {
                GenerateLocationWithApi = true;
                return true;
            }
           
            GenerateLocationWithApi = false;
            return true;
        }


        public bool GenerateLocation()
        {
            return GenerateLocationWithApi;
        }
    }
}
