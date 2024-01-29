

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands
{
    public class ImageCommands : IImageCommands
    {
        private readonly ExpresoDbContext _context;

        public ImageCommands(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task<Image> InsertImage(Image image)
        {
           await _context.AddAsync(image);

           await _context.SaveChangesAsync();

            return image;
        }

        public async Task UpdateImage(Image image)
        {
            Image updated = await _context.Images.FirstOrDefaultAsync(x => x.ImageId == image.ImageId);

            updated.Url = image.Url;

            updated.Order = image.Order;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteImage(Image image)
        {
             _context.Images.Remove(image);

            await _context.SaveChangesAsync();
        }

    }
}
