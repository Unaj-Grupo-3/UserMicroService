

using Application.Models;

namespace Application.Interfaces
{
    public interface IImageServices
    {       
        Task<IList<ImageResponse>> UpdateImages(int userId, IList<int> images);

        Task<ImageResponse> UploadImage(int userId, string url);
    }
}
