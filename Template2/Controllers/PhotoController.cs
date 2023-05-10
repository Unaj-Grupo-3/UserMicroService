using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentacion.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IImageServices _imageServices;
        private readonly IValidateImageServices _validateImageServices;
        private readonly IServerImagesApiServices _serverImagesApiServices;

        public PhotoController(IServerImagesApiServices imgbbApiServices, IUserServices userServices,
                              IImageServices imageServices, IValidateImageServices validateImageServices)
        {
            _userServices = userServices;
            _imageServices = imageServices;
            _validateImageServices = validateImageServices;
            _serverImagesApiServices = imgbbApiServices;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddImage(IFormFile file)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                var userExist = await _userServices.GetUserById(userId);

                if (userExist == null)
                {
                    return new JsonResult(new { Message = $"No existe el usuario con el id {userId}" }) { StatusCode = 404 };
                }

                if (!await _validateImageServices.Validate(file, userId))
                {
                    return new JsonResult(_validateImageServices.GetErrors()) { StatusCode = 400 };
                }

                bool uploadIsValid = await _serverImagesApiServices.UploadImage(file, userId);

                if (!uploadIsValid)
                {
                    return new JsonResult(new { Message = _serverImagesApiServices.GetMessage(), Response = _serverImagesApiServices.GetResponse() }) { StatusCode = _serverImagesApiServices.GetStatusCode() };

                }

                var url = _serverImagesApiServices.GetResponse().RootElement.GetProperty("link").ToString();

                await _imageServices.UploadImage(userId, url);

                var userResponse = await _userServices.GetUserById(userId);

                return new JsonResult(new { Message = "La foto se ha subido correctamente", Response = userResponse }) { StatusCode = 201 };


            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

    }
}
