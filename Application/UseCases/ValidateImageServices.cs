using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace Application.UseCases
{
    public class ValidateImageServices : IValidateImageServices
    {
        public Dictionary<string, string> errors { get; set; }

        private readonly IImageQueries _imageQueries;

        public ValidateImageServices(IImageQueries imageQueries)
        {
            errors = new Dictionary<string, string>();
            _imageQueries = imageQueries;
        }

        public async Task<bool> Validate(IFormFile photo, Guid userId)
        {
                if ((await _imageQueries.GetImageByUserId(userId)).Count >= 6)
                {
                    errors.Add("imagen", "No se pueden agregar mas fotos");
                    return false;
                }

               List<string> extensions =new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".avif", ".webp" };

                string extensionFromPhoto = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (!extensions.Contains(extensionFromPhoto))
                {
                    errors.Add("imagen", "El archivo no esta en el formato correcto");
                    return false;
                }

                if (photo.Length > 5 * 1024 * 1024) // 5 MB
                {
                    errors.Add("imagen", "La imagen no puede pesar más de 5 MB");
                    return false;
                }

                return true;    

        }

        public async Task<bool> ValidateImages(IList<int> images, Guid userId)
        {
            foreach (int imageId in images)
            {
                Image imageByid = await _imageQueries.GetImageById(imageId);

                if (imageByid == null || imageByid.UserId != userId) 
                {
                    errors.Add("imagen", "Error con las fotos ingresadas");
                    return false;
                }

            }

            if (images.Count > 6)
            {
                errors.Add("imagen", "No se pueden agregar mas fotos");
                return false;
            }

            return true;
        }

        public Dictionary<string, string> GetErrors()
        {
            return errors;
        }
    }
}
