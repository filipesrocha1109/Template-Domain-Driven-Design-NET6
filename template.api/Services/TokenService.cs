using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using template.domain.DTOs.Users;

namespace template.api.Services
{
    public class TokenService
    {
        #region FIELDS
        private IConfiguration _configuration;
        #endregion

        #region CTOR
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region PUBLIC METHODS
        public string GenerateToken(UserAuthDTO user)
        {
            #region VARIABLES
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings")["JWT:Secret"].ToString());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(double.Parse(_configuration.GetSection("AppSettings")["JWT:Expiration"].ToString())),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            #endregion   
        }

        public UserAuthDTO? DecriptToken(string token)
        {
            #region VARIABLES
            var handler = new JwtSecurityTokenHandler();
            #endregion

            #region VALIDATIONS
            if (String.IsNullOrEmpty(token))
                return null;
            #endregion

            #region LOGIC
            try
            {
                var jwt = handler.ReadJwtToken(token);

                return new UserAuthDTO()
                {
                    Id  = int.Parse(jwt.Claims.First(claim => claim.Type == "nameid").Value),
                    Name = jwt.Claims.First(claim => claim.Type == "name").Value,
                    Role = jwt.Claims.First(claim => claim.Type == "role").Value
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            #endregion  
        }
        #endregion

        #region PRIVATE METHODS
        #endregion
    }
}
