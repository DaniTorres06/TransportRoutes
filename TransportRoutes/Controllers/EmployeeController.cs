using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportRoutesBusiness;
using TransportRoutesBusiness.Interfaces;
using TransportRoutesModel;
using TransportRoutesModel.ModelView;

namespace TransportRoutes.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeBusiness _service;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeBusiness employeeBusiness)
        {
            _logger = logger;
            _service = employeeBusiness;

        }

        [HttpPost("ValidUserAsync")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseList<Employee>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseList<Employee>>> ValidUserAsync(string User, string Pass)
        {
            try
            {
                var response = await _service.ValidUser(User, Pass);
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
