using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IValidateServices _validateServices;
        private readonly IImageServices _imageServices;
        private readonly IValidateImageServices _validateImageServices;

        public UserController(IUserServices userServices, IValidateServices validateServices, IImageServices imageServices, IValidateImageServices validateImageServices)
        {
            _userServices = userServices;
            _validateServices = validateServices;
            _imageServices = imageServices;
            _validateImageServices = validateImageServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserReq req)
        {
            try
            {
                var diccio = _validateServices.Validate(req).Result;

                foreach (var url in req.Images)
                {
                    var errorImage = await _validateImageServices.ValidateUrl(url);

                    if (!errorImage)
                    {
                        return new JsonResult(_validateImageServices.GetErrors()) { StatusCode = 400 };
                    }
                }

                if (diccio.ElementAt(0).Key)
                {
                    var response = await _userServices.AddUser(req);

                    var images = await _imageServices.AddImages(response.UserId,req.Images);

                    response.Images = images;

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
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }

        }
    }
}
