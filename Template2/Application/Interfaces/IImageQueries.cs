
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IImageQueries
    {
        Task<IList<Image>> GetImageByUserId(int userId);
    }
}
