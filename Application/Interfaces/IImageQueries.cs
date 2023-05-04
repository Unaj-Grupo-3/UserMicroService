
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IImageQueries
    {
        Task<IList<Image>> GetImageByUserId(Guid userId);

        Task<Image> GetImageById(int id);
    }
}
