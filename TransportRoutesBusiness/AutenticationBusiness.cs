using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TransportRoutesData.Interfaces;
using TransportRoutesModel.ModelView;
using TransportRoutesModel;
using TransportRoutesBusiness.Interfaces;

namespace TransportRoutesBusiness
{
    public class AutenticationBusiness : IAutenticationBusiness
    {
        private readonly string secretKey;
        private readonly IConfiguration _config;
        private readonly ILogger<AutenticationBusiness> _logger;        
        private readonly IEmployeeBusiness _employeeBusiness;

        public AutenticationBusiness(   IConfiguration config,
                                        ILogger<AutenticationBusiness> logger,                                        
                                        IEmployeeBusiness employeeBusiness
                                    )
        {
            secretKey = config.GetSection("Settings").GetSection("SecretKey").ToString();
            _config = config;
            _logger = logger;            
            _employeeBusiness = employeeBusiness;
        }

        /*
        public bool atuentication(string user)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokencreado = tokenHandler.WriteToken(tokenConfig);


            //return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });
            return true;
        }
        */

        public async Task<ResponseList<Autentication>> Atuentication(string User, string Pass)
        {
            ResponseList<Employee> responseList = new ResponseList<Employee>();
            ResponseList<Autentication> rspAtuent = new ResponseList<Autentication>();
            Autentication vObjAutent = new Autentication();

            responseList = await _employeeBusiness.ValidUser(User, Pass);
            string tokencreado = string.Empty;

            if (responseList.response.status)
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, User));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                tokencreado = tokenHandler.WriteToken(tokenConfig);
                vObjAutent.Token = tokencreado;
                rspAtuent.List.Add(vObjAutent);
                rspAtuent.response.status = true;
                rspAtuent.response.message = "Token creado con exito";

                return rspAtuent;
            }
            else
            {
                rspAtuent.List.Add(vObjAutent);
                rspAtuent.response.status = false;
                rspAtuent.response.message = "Usuario y/o contraseña incorrecto";
                rspAtuent.Errors = responseList.Errors;

                return rspAtuent;
            }


            //return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });

        }

    }
}
