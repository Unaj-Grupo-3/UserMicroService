
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IImageQueries
    {
        Task<IList<Image>> GetImageByUserId(int userId);

        Task<Image> GetImageById(int id);
    }
}
