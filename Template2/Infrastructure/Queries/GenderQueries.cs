using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistance;


namespace Infrastructure.Queries
{
    public class GenderQueries : IGenderQueries
    {
        private readonly ExpresoDbContext _context;

        public GenderQueries (ExpresoDbContext context)
        {
            _context = context;
        }
        public async Task<Gender> GetDescGenderById(int GenderId)
        {
            var gender =  (from g in  _context.Gender
                                     where g.GenderId == GenderId
                                     select g).FirstOrDefault();
            return gender;
        }

        public async Task<List<Gender>> GetGenders()
        {
            var genders = (from g in _context.Gender
                               select g).ToList();
            return genders;
        }
    }
}
