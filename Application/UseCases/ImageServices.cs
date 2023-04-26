
using Application.Interfaces;
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

        public async Task<IList<string>> AddImages(int userId, IList<string> images)
        {
            List<string> response = new List<string>();

            for (int i = 0; i < images.Count; i++)
            {
                Image image = new Image
                {
                   UserId = userId,
                   Url = images[i],
                   Order = i
                };

               await _commands.InsertImage(image);

                response.Add(images[i]);
            }

            return response;
        }

        public async Task<IList<string>> UpdateImages(int userId, IList<string> images)
        {
            IList<Image> imagesByUserId = await _queries.GetImageByUserId(userId);

            // No hay que crear ni eliminar solo actualizar los links
            if (imagesByUserId.Count == images.Count)
            {
                for(int i = 0;i < imagesByUserId.Count; i++)
                {
                    Image image = new Image
                    {
                        ImageId = imagesByUserId[i].ImageId,
                        Order = imagesByUserId[i].Order,
                        Url = images[i]
                    };

                    await _commands.UpdateImage(image);
                }
            }

            // Habria que eliminar las fotos que sobran
            else if(imagesByUserId.Count > images.Count)
            {
                // Actualizo las anteriores
                for (int i = 0; i < images.Count; i++)
                {
                    Image image = new Image
                    {
                        ImageId = imagesByUserId[i].ImageId,
                        Order = imagesByUserId[i].Order,
                        Url = images[i]
                    };

                    await _commands.UpdateImage(image);
                }

                //Elimino las que sobran
                for (int i = images.Count; i < imagesByUserId.Count; i++)
                {
                    await _commands.DeleteImage(imagesByUserId[i]);
                }
            }

            // Habria que crear las fotos que faltan
            else
            {
                for (int i = 0; i < imagesByUserId.Count; i++)
                {
                    Image image = new Image
                    {
                        ImageId = imagesByUserId[i].ImageId,
                        Order = imagesByUserId[i].Order,
                        Url = images[i]
                    };

                    await _commands.UpdateImage(image);
                }

                for (int i = imagesByUserId.Count; i<images.Count; i++)
                {
                    Image image = new Image
                    {
                        UserId = userId,
                        Order = i,
                        Url = images[i] 
                    };

                    await _commands.InsertImage(image);
                }
            }

            imagesByUserId = await _queries.GetImageByUserId(userId);

            IList<string> response  = new List<string>();

            foreach (Image image in imagesByUserId)
            {
                response.Add(image.Url);
            }

            return response;
        }
    }
}
