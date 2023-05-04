
using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Application.UseCases
{
    public class ImageServices : IImageServices
    {

        private readonly IImageCommands _commands;
        private readonly IImageQueries _queries;

        public ImageServices(IImageCommands commands, IImageQueries queries)
        {
            _commands = commands;
            _queries = queries;
        }

        public async Task<IList<ImageResponse>> UpdateImages(Guid userId, IList<int> images)
        {
            IList<Image> imagesByUserId = await _queries.GetImageByUserId(userId);
            IList<Image> imagesToUpdate = new List<Image>();
            IList<Image> imagesToDelete = new List<Image>();
            images = images.Distinct().ToList();

            // Podria buscar del primer array
            foreach(int imageUpdatedId in images)
            {
                var imageToUpdate = imagesByUserId.FirstOrDefault(x => x.ImageId == imageUpdatedId);

                imagesToUpdate.Add(imageToUpdate);
            }

            foreach (var imageToDelete in imagesByUserId)
            {
                if (!images.Contains(imageToDelete.ImageId))
                {
                    imagesToDelete.Add(imageToDelete);
                }
            }

            // Primero actualizo
            for (int i = 0; i < imagesToUpdate.Count; i++)
            {
                Image image = new Image
                {
                    ImageId = imagesToUpdate[i].ImageId,
                    Order = i,
                    Url = imagesToUpdate[i].Url
                };

                await _commands.UpdateImage(image);
            }

            // Luego elimino
            for (int i = 0; i < imagesToDelete.Count; i++)
            { 
                await _commands.DeleteImage(imagesToDelete[i]);
            }

            imagesByUserId = await _queries.GetImageByUserId(userId);

            IList<ImageResponse> response  = new List<ImageResponse>();

            foreach (Image image in imagesByUserId)
            {
                ImageResponse imageResponse = new ImageResponse
                {
                    Id = image.ImageId,
                    Url = image.Url,
                };

                response.Add(imageResponse);
            }

            return response;
        }

        public async Task<ImageResponse> UploadImage(Guid userId, string url)
        {
            IList<Image> imagesByUserId = await _queries.GetImageByUserId(userId);

            Image image = new Image
            {
                UserId = userId,
                Url = url,
                Order = imagesByUserId.Count
            };

            Image create = await _commands.InsertImage(image);

            ImageResponse response = new ImageResponse()
            {
                Id = image.ImageId,
                Url = image.Url,
            };

            return response;
        }
    }
}
