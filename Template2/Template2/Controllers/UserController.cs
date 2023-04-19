using Application.Interfaces;
using Application.Models;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IValidateServices _validateServices;

        public UserController(IUserServices userServices, IValidateServices validateServices)
        {
            _userServices = userServices;
            _validateServices = validateServices;
        }

        [HttpPost]
        public async Task<IActionResult> User(UserReq req)
        {
            try
            {
                var diccio = _validateServices.Validate(req).Result;

                if (diccio.ElementAt(0).Key)
                {
                    var response = await _userServices.AddUser(req);
                    return new JsonResult(new { Message = "Se ha creado el usuario exitosamente.", Response = response }) { StatusCode = 201 };
                }
                else
                {
                    var errores = diccio.ElementAt(0).Value;
                    return new JsonResult(new { Message = "Existen errores en la petición.", Response = errores }) { StatusCode = 400 };
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor."}) { StatusCode = 500 };
            }
                


        }
    }
}
