

using Application.Models;

namespace Application.Interfaces
{
    public interface IImageServices
    {       
        Task<IList<ImageResponse>> UpdateImages(Guid userId, IList<int> images);

        Task<ImageResponse> UploadImage(Guid userId, string url);
    }
}
