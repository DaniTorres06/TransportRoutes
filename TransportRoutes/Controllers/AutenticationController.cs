using Microsoft.AspNetCore.Mvc;
using TransportRoutesBusiness.Interfaces;
using TransportRoutesModel.ModelView;
using TransportRoutesModel;

namespace TransportRoutes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticationController : Controller
    {
        private readonly ILogger<AutenticationController> _logger;
        private readonly IAutenticationBusiness _service;

        public AutenticationController(ILogger<AutenticationController> logger, IAutenticationBusiness autenticationBusiness)
        {
            _logger = logger;
            _service = autenticationBusiness;

        }

        [HttpPost("AutenticationAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseList<Autentication>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseList<Autentication>>> AutenticationAsync(string User, string Pass)
        {
            try
            {
                var response = await _service.Atuentication(User, Pass);

                /*
                if (response != string.Empty)
                {
                    return StatusCode(StatusCodes.Status200OK, new { token = response });
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
                }
                */
                
                if (response is null)
                    return BadRequest();

                if (!response.response.status)
                    return BadRequest(response);
                else                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
