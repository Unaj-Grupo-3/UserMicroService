
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IImageCommands
    {
        public Task InsertImage(Image image);
    }
}
