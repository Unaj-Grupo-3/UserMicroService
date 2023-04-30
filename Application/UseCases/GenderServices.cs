using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Application.UseCases
{
    public class GenderServices : IGenderServices
    {
        private readonly IGenderQueries _queries;

        public GenderServices(IGenderQueries queries)
        {
            _queries = queries;
        }

        public async Task<GenderResponse> GetDescGenderById(int GenderId)
        {
            var query = await _queries.GetDescGenderById(GenderId);
            if(query != null)
            {
                GenderResponse gender = new GenderResponse
                {
                    GenderId = query.GenderId,
                    Description = query.Description
                };
                return gender;
            }
            return null;
        }

        public async Task<List<GenderResponse>> GetGenders()
        {
            var genders = await _queries.GetGenders();
            if(genders != null)
            {
                List<GenderResponse> gendersResponse = new List<GenderResponse>();

                foreach (Gender gender in genders)
                {
                    gendersResponse.Add(new GenderResponse
                    {
                        GenderId = gender.GenderId,
                        Description = gender.Description
                    });
                }

                return gendersResponse;
            }
            return null;
            
        }
    }
}
