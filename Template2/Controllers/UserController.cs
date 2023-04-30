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
        private readonly IValidateServices _validateServices;
        private readonly IImageServices _imageServices;
        private readonly IValidateImageServices _validateImageServices;
        private readonly IAuthApiServices _authApiServices;
        private readonly IServerImagesApiServices _serverImagesApiServices;
        private readonly IGenderServices _genderServices;

        public UserController(IServerImagesApiServices imgbbApiServices,
                              IUserServices userServices, 
                              IValidateServices validateServices, 
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

        // Quizas se necesite autenticacion?
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserById()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

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

        // Tendria que ser creado en el momento que se crea un auth con valores predeterminados.(Crear en el micro de auth)
        [HttpPost]
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

                if (diccio.ElementAt(0).Key)
                {

                    bool postIsValid = await _authApiServices.CreateAuth(req.AuthReq);

                    if (!postIsValid)
                    {
                        return new JsonResult(new { Message = _authApiServices.GetMessage(), Response = _authApiServices.GetResponse() }) { StatusCode = _authApiServices.GetStatusCode() };
                    }

                    Guid authId = Guid.Parse(_authApiServices.GetResponse().RootElement.GetProperty("id").GetString());

                    var response = await _userServices.AddUser(req, authId);

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

        // Agregar autenticacion con JWT
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser(UserReq req)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

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
                    IList<string> imagesUpdate = new List<string>();

                    if (req.Images != null && req.Images.Count != 0)
                    {
                        if(!await _validateImageServices.ValidateImages(req.Images, userId))
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

        [HttpGet("api/users/{name?}")]
        public async Task<IActionResult> GetAll(string? name=null)
        {
            try
            {
                var users = await _userServices.GetByFirstName(name);

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

        // Agregar autenticacion con JWT. Devolver User en la response.
        [HttpPost("{id}/Photo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddImage(int id,IFormFile file)
        {
            try
            {
                var userExist = await _userServices.GetUserById(id);

                if (userExist == null)
                {
                    return new JsonResult(new { Message = $"No existe el usuario con el id {id}" }) { StatusCode = 404 };
                }

                if (!await _validateImageServices.Validate(file,id))
                {
                    return new JsonResult(_validateImageServices.GetErrors()) { StatusCode = 400 };
                }

                bool uploadIsValid = await _serverImagesApiServices.UploadImage(file, id);

                if (!uploadIsValid)
                {
                    return new JsonResult(new { Message = _serverImagesApiServices.GetMessage(), Response = _serverImagesApiServices.GetResponse() }) { StatusCode = _serverImagesApiServices.GetStatusCode() };

                }

                var url = _serverImagesApiServices.GetResponse().RootElement.GetProperty("link").ToString();

                await _imageServices.UploadImage(id, url);

                var userResponse = await _userServices.GetUserById(id);

                return new JsonResult(new { Message = "La foto se ha subido correctamente", Response = userResponse }) { StatusCode = 201 };


            }catch(Exception)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }
    }
}
