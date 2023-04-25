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
        private readonly IAuthApiServices _authApiServices;

        public UserController(IUserServices userServices, IValidateServices validateServices, IImageServices imageServices, IValidateImageServices validateImageServices, IAuthApiServices authApiServices)
        {
            _userServices = userServices;
            _validateServices = validateServices;
            _imageServices = imageServices;
            _validateImageServices = validateImageServices;
            _authApiServices = authApiServices;
        }

        // Quizas se necesite autenticacion?
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                UserResponse response = await _userServices.GetUserById(id);

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

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserReq req)
        {
            try
            {
                var diccio = _validateServices.Validate(req, false).Result;

                if (req.Images == null || req.Images.Count == 0)
                {
                    return new JsonResult(new { Message = "Es necesario ingresar una foto." }) { StatusCode = 400 };
                }

                if (req.Images.Count > 6)
                {
                    return new JsonResult(new { Message = "Solo se permiten 6 fotos." }) { StatusCode = 400 };
                }

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

                    bool postIsValid = await _authApiServices.CreateAuth(req.AuthReq);

                    if (!postIsValid)
                    {
                        return new JsonResult(new { Message = _authApiServices.GetMessage(), Response = _authApiServices.GetResponse() }) { StatusCode = _authApiServices.GetStatusCode() };
                    }

                    Guid authId = Guid.Parse(_authApiServices.GetResponse().RootElement.GetProperty("id").GetString());

                    var response = await _userServices.AddUser(req, authId);
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


        // Agregar autenticacion
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,UserReq req)
        {
            try
            {
                var userExist = await _userServices.GetUserById(id);

                if (userExist == null)
                {
                    return new JsonResult(new {Message = $"No existe el usuario con el id {id}"}) { StatusCode = 404 };   
                }

                var diccio = _validateServices.Validate(req, true).Result;

                var algo = diccio.ElementAt(0).Key;

                if (diccio.ElementAt(0).Key)
                {
                    IList<string> imagesUpdate = new List<string>();

                    if (req.Images != null)
                    {
                        if (req.Images.Count == 0)
                        {
                            return new JsonResult(new { Message = "Es necesario ingresar una foto." }) { StatusCode = 400 };
                        }

                        if (req.Images.Count > 6)
                        {
                            return new JsonResult(new { Message = "Solo se permiten 6 fotos." }) { StatusCode = 400 };
                        }

                        foreach (var url in req.Images)
                        {
                            var errorImage = await _validateImageServices.ValidateUrl(url);

                            if (!errorImage)
                            {
                                return new JsonResult(_validateImageServices.GetErrors()) { StatusCode = 400 };
                            }
                        }

                        imagesUpdate = await _imageServices.UpdateImages(id, req.Images);
                    }

                    var response = await _userServices.UpdateUser(id, req);

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
            catch (Microsoft.Data.SqlClient.SqlException)
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

        // Usado en el micro de auth para generar el token
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
    }
}
