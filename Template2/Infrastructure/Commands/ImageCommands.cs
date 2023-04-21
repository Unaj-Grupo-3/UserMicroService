

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;

namespace Infrastructure.Commands
{
    public class ImageCommands : IImageCommands
    {
        private readonly ExpresoDbContext _context;

        public ImageCommands(ExpresoDbContext context)
        {
            _context = context;
        }

        public async Task InsertImage(Image image)
        {
           await _context.AddAsync(image);

           await _context.SaveChangesAsync();
        }
    }
}
