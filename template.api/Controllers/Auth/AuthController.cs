using Microsoft.AspNetCore.Mvc;
using template.crosscutting.Utils;
using template.domain.Common;
using template.domain.Common.HttpHandle.Request;
using template.domain.Enums;
using template.domain.Interfaces.Auth;
using template.api.Services;
using template.domain.Common.HttpHandle.Response;
using template.domain.DTOs.Users;

namespace template.api.Controllers.Auth
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        #region FIELDS
        private readonly IAuthService _authService;
        private IConfiguration _configuration;
        private TokenService _tokenService;
        #endregion

        #region CTOR
        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
            _tokenService = new TokenService(_configuration);
        }
        #endregion

        #region PUBLIC METHODS
        [HttpPost("Token")]
        public async Task<ActionResult<Response>> Token(AuthRequest request)
        {
            #region VARIABLES
            var response = new Response();
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                response = await _authService.Token(request);

                if (!response.Success)
                    return StatusCode(response.Status, response);

                var token = new AuthResponse()
                {
                    Token = new Token()
                    {
                        Expires_in = long.Parse(_configuration.GetSection("AppSettings")["JWT:Expiration"].ToString()),
                        Access_token = _tokenService.GenerateToken((UserAuthDTO)response.Data),
                        Token_type = _configuration.GetSection("AppSettings")["JWT:Type"].ToString(),
                    }
                };

                response.Data = new
                {
                    user = (UserAuthDTO)response.Data,
                    token = token
                };

                return StatusCode(response.Status, response);
            }
            catch (Exception e)
            {
                return StatusCode((int)ReturnStatus.InternalServerError, response = new()
                {
                    Message = $"MESSAGE => {e.Message} || INNER EXCEPTION => {e.InnerException}",
                    Code = (int)ReturnCodes.ExceptionEx,
                    Success = false
                });
            }
            #endregion
        }
        #endregion

        #region PRIVATE METHODS
        #endregion
    }
}
