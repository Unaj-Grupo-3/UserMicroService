using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IValidateServices
    {
        Task<bool> ValidateLenght(string verify, string tag, int maxLenght);
        Task<bool> ValidateCharacters(string verify, string tag);
        Task<IDictionary<bool, IDictionary<string, string>>> Validate(UserReq userReq);
    }
}
