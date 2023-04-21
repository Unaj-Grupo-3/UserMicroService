
using Application.Models;


namespace Application.Interfaces
{
    public interface IImageServices
    {
        public Task<IList<string>> AddImages(int userId, IList<string> images);
        
    }
}
