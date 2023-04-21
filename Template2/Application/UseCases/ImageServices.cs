
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
    }
}
