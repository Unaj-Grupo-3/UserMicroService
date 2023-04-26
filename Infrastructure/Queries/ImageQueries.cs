

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class ImageQueries : IImageQueries
    {
        private readonly ExpresoDbContext _context;

        public ImageQueries(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task<Image> GetImageById(int id)
        {
            Image image = await _context.Images.FirstOrDefaultAsync(x => x.ImageId == id);

            return image;
        }

        public async Task<IList<Image>> GetImageByUserId(int userId)
        {
            IList<Image> images = await _context.Images.Where(x => x.UserId == userId).OrderBy(x => x.Order).ToListAsync();

            return images;
        }
    }
}
