using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransportRoutesData.Interfaces;
using TransportRoutesModel;
using TransportRoutesModel.ModelView;

namespace TransportRoutesData
{
    public class EmployeeData : IEmployeeData
    {
        private readonly ILogger<EmployeeData> _logger;
        private readonly IAccessData _accessData;
        private readonly IConfiguration _config;

        private const string Validate_User = "sp_validate_user";

        public EmployeeData(ILogger<EmployeeData> logger, IAccessData accessData, IConfiguration configuration)
        {
            _logger = logger;
            _accessData = accessData;
            _config = configuration;
        }

        public async Task<ResponseList<Employee>?> Validateuser(string User, string Pass)
        {

            List<SqlParameter> sqlParameters = new();
            sqlParameters.Add(new SqlParameter("@user", User));
            sqlParameters.Add(new SqlParameter("@password", Pass));

            return await _accessData.ExecuteReaderSpEAsync<Employee>(Validate_User, sqlParameters);

        }


    }
}
