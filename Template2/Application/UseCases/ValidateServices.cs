using Application.Interfaces;
using Application.Models;
using System.Text.RegularExpressions;


namespace Application.UseCases
{
    public class ValidateServices : IValidateServices
    {
        public Dictionary<string, string> errors { get; set; }

        public ValidateServices() 
        {
            errors = new Dictionary<string, string>();        
        }

        public async Task<IDictionary<bool, IDictionary<string, string>>> Validate(UserReq userReq, bool allowNull)
        {
            bool nombre = true;

            if (!allowNull || userReq.Name != null)
            {
                nombre = await ValidateLenght(userReq.Name, "Nombre", 50) && await ValidateCharacters(userReq.Name, "Nombre");
            }

            bool birthday = true;

            if (!allowNull || userReq.Birthday != null)
            {
                birthday = await ValidateAge(userReq.Birthday, "Fecha de Nacimiento");
            }

            bool apellido = true;

            if (!allowNull || userReq.LastName != null)
            {
                apellido = await ValidateLenght(userReq.LastName, "Apellido", 50) && await ValidateCharacters(userReq.LastName, "Apellido");
            }

            bool descripcion = true;

            if (!allowNull || userReq.Description != null)
            {
                descripcion = await ValidateLenght(userReq.Description, "Descripcion", 255);
            }

            IDictionary<bool, IDictionary<string, string>> newDictionary = new Dictionary<bool, IDictionary<string, string>>() { };

            if (nombre && apellido && descripcion && birthday)
            {
                newDictionary.Add(true, errors);
                return newDictionary;
            }
            else
            {
                newDictionary.Add(false, errors);
                return newDictionary;
            }
        }

        public async Task<bool> ValidateLenght(string verify, string tag, int maxLenght)
        {
            string errorMessage = "";
            bool isValid = true;

            if (verify == null || verify.Trim() == "")
            {
                errorMessage = "No se ingresó ningún dato en uno de los campos.";
                isValid = false;
            }

            if (verify != null && verify.Length > maxLenght)
            {
                errorMessage = $"La cadena ingresada en uno de los campos es muy larga. Máximos caracteres permitidos: {maxLenght}.";
                isValid = false;
            }

            if (!errors.ContainsKey(tag) && !isValid)
            {
                errors.Add(tag, errorMessage);
            }

            return isValid;
        }

        public async Task<bool> ValidateCharacters(string verify, string tag)
        {
            string errorMessage = "";
            bool isValid = true;

            if (!Regex.IsMatch(verify, @"^[A-Za-zÀ-ÿ_\s]+$"))
            {
                errorMessage = "Caracteres prohibidos.";
                isValid = false;
            }

            if (!errors.ContainsKey(tag) && !isValid)
            {
                errors.Add(tag, errorMessage);
            }

            return isValid;
        }

        public async Task<bool> ValidateAge(DateTime? verify, string tag)
        {

            if (verify == null)
            {
                errors.Add(tag, "No se ingresó ningún dato en uno de los campos.");
                return false;
            }

            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - verify.Value.Year;
            if (verify.Value > hoy.AddYears(-edad)) edad--;

            if (edad < 18)
            {
                errors.Add(tag, "Debes tener mas de 18 años.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
