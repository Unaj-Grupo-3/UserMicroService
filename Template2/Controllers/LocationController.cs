using Application.Interfaces;
using Application.Models;
using Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Presentacion.Controllers
{
    [Route("api/v1/&[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationApiServices _service;

        public LocationController(ILocationApiServices service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocation([FromQuery]string location)
        {
            try
            {
                var jsonAddress = await _service.GetLocation(location);

                return Ok(jsonAddress);
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                return new JsonResult(new { Message = "Se ha producido un error interno en el servidor." }) { StatusCode = 500 };
            }
        }
    }
}
