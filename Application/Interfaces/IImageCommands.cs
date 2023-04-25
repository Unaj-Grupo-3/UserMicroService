
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IImageCommands
    {
        Task InsertImage(Image image);

        Task UpdateImage(Image image);

        Task DeleteImage(Image image);
    }
}
