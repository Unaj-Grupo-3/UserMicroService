using Abp.Webhooks;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Azure.Core;
using Presentation.Authorization;
using System.Security.Cryptography;

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
        private readonly IServerImagesApiServices _serverImagesApiServices;
        private readonly IGenderServices _genderServices;
        private readonly IValidateLocationServices _validateLocationServices;
        private readonly ILocationServices _locationServices;
        private readonly ILocationApiServices _locationApiServices;
        private readonly IConfiguration _configuration;

        public UserController(IServerImagesApiServices imgbbApiServices,
                              IUserServices userServices, 
                              IValidateUserServices validateServices,
                              IImageServices imageServices,
                              IValidateImageServices validateImageServices,
                              IGenderServices genderServices,
                              IValidateLocationServices validateLocationServices,
                              ILocationServices locationServices,
                              ILocationApiServices locationApiServices,
                              IConfiguration configuration)
        {
            _userServices = userServices;
            _validateServices = validateServices;
            _imageServices = imageServices;
            _validateImageServices = validateImageServices;
            _serverImagesApiServices = imgbbApiServices;
            _genderServices = genderServices;
            _validateLocationServices = validateLocationServices;
            _locationServices = locationServices;
            _locationApiServices = locationApiServices;
            _configuration = configuration;
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = ApiKeySchemeOptions.Scheme)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var apikey = _configuration.GetSection("ApiKey").Get<string>();
                var key = HttpContext.User.Identity.Name;

                if (key != null && key != apikey)
                {
                    return new JsonResult(new { Message = "No se puede acceder. La Key es inválida" }) { StatusCode = 401 };
                }
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

        //[HttpGet("me")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> GetUserById()
        //{
        //    try
        //    {
        //        var identity = HttpContext.User.Identity as ClaimsIdentity;
        //        int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

        //        UserResponse response = await _userServices.GetUserById(userId);

        //        if (response == null)
        //        {
        //            return new JsonResult(new {Message = "No tiene un perfil creado"}) { StatusCode = 404};
        //        }

        //        return new JsonResult(response);
        //    }
        //    catch (Microsoft.Data.SqlClient.SqlException)
        //    {
        //        return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
        //    }
        //}


        [HttpGet("{fullResponse}")]
        [Authorize(AuthenticationSchemes = ApiKeySchemeOptions.Scheme)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllListUsers([FromQuery] List<int> usersId, bool fullResponse)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var apikey = _configuration.GetSection("ApiKey").Get<string>();
                var key = HttpContext.User.Identity.Name;

                if(key != null && key != apikey)
                {
                    return new JsonResult(new { Message = "No se puede acceder. La Key es inválida" }) { StatusCode = 401 };
                }

                //Lista vacia, ApiKey NO, JWT SI -> (getMyUser)
                if(!usersId.Any() && HttpContext.User.Identity.Name == null && identity.Claims.Any())
                {
                    int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);             

                    UserResponse response = await _userServices.GetUserById(userId);

                    if (response == null)
                    {
                        return new JsonResult(new { Message = "No tiene un perfil creado" }) { StatusCode = 404 };
                    }

                    return new JsonResult(response);
                }

                if (fullResponse)
                {
                    var usersFull = await _userServices.GetAllUsersFullByIds(usersId);
                    if (usersFull == null)
                    {
                        return NotFound();
                    }

                    return new JsonResult(usersFull);
                }
                else
                {
                   var users = await _userServices.GetAllUsersByIds(usersId);
                    if (users == null)
                    {
                        return NotFound();
                    }

                    return new JsonResult(users);
                }

            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateUser(UserReq req)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int userId = int.Parse(identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value);

                var getUserById = await _userServices.GetUserById(userId);

                if (getUserById != null)
                {
                    return new JsonResult(new { Message = "Ya estas registrado con esta cuenta!"}) { StatusCode = 400 };
                }

                var diccio = _validateServices.Validate(req, false).Result;

                if (!diccio.ElementAt(0).Key)
                {
                    var errores = diccio.ElementAt(0).Value;
                    return new JsonResult(new { Message = "Existen errores en la petición.", Response = errores }) { StatusCode = 400 };

                }

                if (req.Gender == null)
                {
                    return new JsonResult(new { Message = $"Ingrese un genero" }) { StatusCode = 400 };
                }

                var genderById = await _genderServices.GetDescGenderById(req.Gender.Value);

                if (genderById == null)
                {
                    return new JsonResult(new { Message = $"No existe un genero con el id {req.Gender.Value}" }) { StatusCode = 404 };
                }


                int locationId;
                // Con o sin coordenadas?
                // Validar si se ingreso correctamente
                if (!_validateLocationServices.ValidateLocation(req.Location)) //Si no se ingreso nada
                {
                    return new JsonResult(new { Message = $"Ingrese una ubicación valida" }) { StatusCode = 400 };
                }
                else if (_validateLocationServices.GenerateLocation()) // Si solo se ingreso la ciudad
                {
                    //Ya existe la ciudad?
                    LocationResponse locationByCity = await _locationServices.GetLocationByCity(req.Location.City);
                    if (locationByCity == null) // No existe esa ciudad
                    {
                        LocationReq locationJson = await _locationApiServices.GetLocation(req.Location.City);
                        LocationResponse locationCreated = await _locationServices.AddLocation(locationJson);
                        locationId = locationCreated.Id;
                    }
                    else // Existe la ciudad
                    {
                        locationId = locationByCity.Id;
                    }
                } //Si se ingresaron todos los datos
                else
                {
                    // Busco por coordenada
                    LocationResponse locationByCoords = await _locationServices.GetLocationByCoords(req.Location.Latitude.Value,req.Location.Longitude.Value);
                    if (locationByCoords == null) // No existe esa coordenada
                    {
                        LocationResponse locationCreated = await _locationServices.AddLocation(req.Location);
                        locationId = locationCreated.Id;
                    }
                    else // Existe la ciudad
                    {
                        locationId = locationByCoords.Id;
                    }
                }

                var response = await _userServices.AddUser(req, userId, locationId);
                return new JsonResult(new { Message = "Se ha creado el usuario exitosamente.", Response = response }) { StatusCode = 201 };

            }
            catch (Microsoft.Data.SqlClient.SqlException)
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

                if (!diccio.ElementAt(0).Key)
                {
                    var errores = diccio.ElementAt(0).Value;
                    return new JsonResult(new { Message = "Existen errores en la petición.", Response = errores }) { StatusCode = 400 };
                }

                int locationId = userExist.Location.Id;
                if (req.Location != null)
                {
                    // Con o sin coordenadas?
                    // Validar si se ingreso correctamente
                    if (!_validateLocationServices.ValidateLocation(req.Location)) //Si no se ingreso nada
                    {
                        return new JsonResult(new { Message = $"Ingrese una ubicación valida" }) { StatusCode = 400 };
                    }
                    else if (_validateLocationServices.GenerateLocation()) // Si solo se ingreso la ciudad
                    {
                        //Ya existe la ciudad?
                        LocationResponse locationByCity = await _locationServices.GetLocationByCity(req.Location.City);
                        if (locationByCity == null) // No existe esa ciudad
                        {
                            LocationReq locationJson = await _locationApiServices.GetLocation(req.Location.City);
                            LocationResponse locationCreated = await _locationServices.AddLocation(locationJson);
                            locationId = locationCreated.Id;
                        }
                        else // Existe la ciudad
                        {
                            locationId = locationByCity.Id;
                        }
                    } //Si se ingresaron todos los datos
                    else
                    {
                        // Busco por coordenada
                        LocationResponse locationByCoords = await _locationServices.GetLocationByCoords(req.Location.Latitude.Value, req.Location.Longitude.Value);
                        if (locationByCoords == null) // No existe esa coordenada
                        {
                            LocationResponse locationCreated = await _locationServices.AddLocation(req.Location);
                            locationId = locationCreated.Id;
                        }
                        else // Existe la ciudad
                        {
                            locationId = locationByCoords.Id;
                        }
                    }
                }

                IList<ImageResponse> imagesUpdate = new List<ImageResponse>();

                if (req.Images != null && req.Images.Count != 0)
                {
                    if (!await _validateImageServices.ValidateImages(req.Images, userId))
                    {
                        return new JsonResult(new { Message = "Error en la carga de fotos", Response = _validateImageServices.GetErrors() }) { StatusCode = 200 };
                    }

                    imagesUpdate = await _imageServices.UpdateImages(userId, req.Images);
                }

                var response = await _userServices.UpdateUser(userId, req, locationId);

                if (imagesUpdate.Count > 0)
                {
                    response.Images = imagesUpdate;
                }

                return new JsonResult(new { Message = "Se ha actualizado el usuario exitosamente.", Response = response }) { StatusCode = 200 };

            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }

    }
}
