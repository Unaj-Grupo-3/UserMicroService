

namespace Application.Interfaces
{
    public interface IImageServices
    {
        Task<IList<string>> AddImages(int userId, IList<string> images);
        
        Task<IList<string>> UpdateImages(int userId, IList<string> images);
    }
}
