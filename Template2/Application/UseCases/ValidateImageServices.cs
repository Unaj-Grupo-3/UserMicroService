
using Application.Interfaces;
using System.Text.RegularExpressions;

namespace Application.UseCases
{
    public class ValidateImageServices : IValidateImageServices
    {
        public Dictionary<string, string> errors { get; set; }

        public ValidateImageServices()
        {
            errors = new Dictionary<string, string>();
        }

        public async Task<bool> ValidateUrl(string url)
        {
            try
            {
                // Validar URL de la imagen
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    errors.Add("imagen", "La URL de la imagen ingresada no es valida");
                    return false;
                }

                string contentType = response.Content.Headers.ContentType.MediaType;
                if (!contentType.StartsWith("image/"))
                {
                    errors.Add("imagen", "La URL no devuelve una imagen");
                    return false;
                }

                var extension = contentType.Split('/')[1];

                var regex = new Regex(@"^\.?(jpg|jpeg|png|gif)$", RegexOptions.IgnoreCase);
                if (!regex.IsMatch(extension))
                {
                    errors.Add("imagen", "La URL no devuelve una imagen con el formato valido");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errors.Add("imagen", "Ha ocurrido un error con la URL");
                return false;
            }

        }

        public Dictionary<string, string> GetErrors()
        {
            return errors;
        }
    }
}
