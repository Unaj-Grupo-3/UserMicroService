

namespace Application.Interfaces
{
    public interface IImageServices
    {       
        Task<IList<string>> UpdateImages(int userId, IList<int> images);

        Task<string> UploadImage(int userId, string url);
    }
}
