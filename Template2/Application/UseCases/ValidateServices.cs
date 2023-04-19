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

        public async Task<IDictionary<bool, IDictionary<string, string>>> Validate(UserReq userReq)
        {
            bool nombre = await ValidateLenght(userReq.Name, "Nombre", 50);
            bool nombre2 = await ValidateCharacters(userReq.Name, "Nombre");
            bool apellido = await ValidateLenght(userReq.LastName, "Apellido", 50);
            bool apellido2 = await ValidateCharacters(userReq.LastName, "Apellido");
            bool descripcion = await ValidateLenght(userReq.Description, "Descripcion", 255);
            IDictionary<bool, IDictionary<string, string>> newDictionary = new Dictionary<bool, IDictionary<string, string>>() { };

            if (nombre && apellido && descripcion && nombre2 && apellido2)
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
            if (verify.Length > maxLenght)
            {
                errors.Add(tag, "La cadena ingresada en uno de los campos es muy larga. Máximos caracteres permitidos: 50.");
                return false;
            }
            if (verify.Trim() == "")
            {
                errors.Add(tag, "No se ingresó ningún dato en uno de los campos.");
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateCharacters(string verify, string tag)
        {
            if(!Regex.IsMatch(verify, @"^[A-Za-zÀ-ÿ_\s]+$"))
            {
                errors.Add(tag, "Caracteres prohibidos.");
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
