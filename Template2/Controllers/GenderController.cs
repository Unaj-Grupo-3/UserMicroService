using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {

        private readonly IGenderServices _genderService;

        public GenderController(IGenderServices genderService)
        {
            _genderService = genderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDescGenderById(int id)
        {
            try
            {
                var response = await _genderService.GetDescGenderById(id);
                if(response == null)
                {
                    return NotFound();
                }
                
                return new JsonResult(response);
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGenders()
        {
            try
            {
                var response = await _genderService.GetGenders();
                if (response == null)
                {
                    return NotFound();
                }

                return new JsonResult(response);
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }
    }
}
