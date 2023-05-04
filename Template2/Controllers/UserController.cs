using Abp.Webhooks;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentacion.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IValidateUserServices _validateServices;
        private readonly IImageServices _imageServices;
        private readonly IValidateImageServices _validateImageServices;
        private readonly IAuthApiServices _authApiServices;
        private readonly IServerImagesApiServices _serverImagesApiServices;
        private readonly IGenderServices _genderServices;

        public UserController(IServerImagesApiServices imgbbApiServices,
                              IUserServices userServices, 
                              IValidateUserServices validateServices, 
                              IImageServices imageServices, 
                              IValidateImageServices validateImageServices, 
                              IAuthApiServices authApiServices,
                              IGenderServices genderServices)
        {
            _userServices = userServices;
            _validateServices = validateServices;
            _imageServices = imageServices;
            _validateImageServices = validateImageServices;
            _authApiServices = authApiServices;
            _serverImagesApiServices = imgbbApiServices;
            _genderServices = genderServices;
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserById()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                Guid userId = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                UserResponse response = await _userServices.GetUserById(userId);

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

        [HttpGet("All")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userServices.GetUserByList();

                if (users == null)
                {
                    return NotFound();
                }

                return new JsonResult(users);

            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

        [HttpGet("userByIds/ids")]
        public async Task<IActionResult> GetAllListUsers([FromQuery] List<Guid> usersId)
        {
            try
            {
                var users = await _userServices.GetAllUserByIds(usersId);

                if (users == null)
                {
                    return NotFound();
                }

                return new JsonResult(users);

            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

        // Usado en el micro de auth para generar el token. Si CreateUser se use en el MICRO-AUTH no hace falta tenerlo.
        [HttpGet("Auth/{authId}")]
        public async Task<IActionResult> GetUserByAuthId(Guid authId)
        {
            try
            {
                UserResponse response = await _userServices.GetUserByAuthId(authId);

                if (response == null)
                {
                    return NotFound();
                }

                return new JsonResult(new { Message = "Encontrado.", Response = response });
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

        // Tendria que ser creado en el momento que se crea un auth con valores predeterminados.(Crear en el micro de auth)
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateUser(UserReq req)
        {
            try
            {
                var diccio = _validateServices.Validate(req, false).Result;

                if (req.Gender == null)
                {
                    return new JsonResult(new { Message = $"Ingrese un genero" }) { StatusCode = 400 };
                }

                var genderById = await _genderServices.GetDescGenderById(req.Gender.Value);

                if (genderById == null)
                {
                    return new JsonResult(new { Message = $"No existe un genero con el id {req.Gender.Value}" }) { StatusCode = 404 };
                }

                if (diccio.ElementAt(0).Key) //validacion[0] true/false
                {

                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    Guid userId = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);
                    Guid authId = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "AuthId").Value);

                    var response = await _userServices.AddUser(req, authId, userId); 

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

        [HttpPost("Photo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddImage(IFormFile file)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                Guid userId = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

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

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser(UserUpdReq req)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                Guid userId = Guid.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                var userExist = await _userServices.GetUserById(userId);

                if (userExist == null)
                {
                    return new JsonResult(new {Message = $"No existe el usuario con el id {userId}"}) { StatusCode = 404 };   
                }

                if (req.Gender != null)
                {
                    var genderById = await _genderServices.GetDescGenderById(req.Gender.Value);

                    if (genderById == null)
                    {
                        return new JsonResult(new { Message = $"No existe un genero con el id {req.Gender.Value}" }) { StatusCode = 404 };
                    }
                }

                var diccio = _validateServices.Validate(req, true).Result;

                var algo = diccio.ElementAt(0).Key;

                if (diccio.ElementAt(0).Key)
                {
                    IList<ImageResponse> imagesUpdate = new List<ImageResponse>();

                    if (req.Images != null && req.Images.Count != 0)
                    {
                        if (!await _validateImageServices.ValidateImages(req.Images, userId))
                        {
                            return new JsonResult(new { Message = "Error en la carga de fotos", Response = _validateImageServices.GetErrors() }) { StatusCode = 200 };
                        }

                        imagesUpdate = await _imageServices.UpdateImages(userId, req.Images);
                    }

                    var response = await _userServices.UpdateUser(userId, req);

                    if (imagesUpdate.Count > 0)
                    {
                        response.Images = imagesUpdate;
                    }

                    return new JsonResult(new { Message = "Se ha actualizado el usuario exitosamente.", Response = response }) { StatusCode = 200 };
                }
                else
                {
                    var errores = diccio.ElementAt(0).Value;
                    return new JsonResult(new { Message = "Existen errores en la petición.", Response = errores }) { StatusCode = 400 };
                }
            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

    }
}
