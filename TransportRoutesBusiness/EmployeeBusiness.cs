using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransportRoutesBusiness.Interfaces;
using TransportRoutesData.Interfaces;
using TransportRoutesModel;
using TransportRoutesModel.ModelView;

namespace TransportRoutesBusiness
{
    public class EmployeeBusiness : IEmployeeBusiness
    {
        private readonly string secretKey;
        private readonly ILogger<EmployeeBusiness> _logger;
        private readonly IEmployeeData _employeeData;

        public EmployeeBusiness(IConfiguration config, ILogger<EmployeeBusiness> logger, IEmployeeData employeeData)
        {
            secretKey = config.GetSection("Settings").GetSection("SecretKey").ToString();
            _logger = logger;
            _employeeData = employeeData;
        }

        public async Task<ResponseList<Employee>?> ValidUser(string User, string Pass)
        {
            try
            {
                return await _employeeData.Validateuser(User, Pass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
