using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IAuthServices _authServices;

        public UserController(IUserServices userServices, IAuthServices authServices)
        {
            _userServices = userServices;
            _authServices = authServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserReq req)
        {
            //¿ Validar request ?
            await _userServices.AddUser(req);

            //Mejorar respuesta
            return new JsonResult(new { result = "Se ha creado el usuario exitosamente." }) { StatusCode = 201 };
        }
    }
}
