using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using template.api.Services;
using template.domain.Common;
using template.domain.Common.HttpHandle.Request;
using template.domain.Enums;
using template.domain.Interfaces.Users;

namespace template.api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        #region FIELDS
        private readonly IUsersService _usersService;
        private IConfiguration _configuration;
        private TokenService _tokenService;
        #endregion

        #region CTOR
        public UsersController(IUsersService usersService, IConfiguration configuration)
        {
            _usersService = usersService;
            _configuration = configuration;
            _tokenService = new TokenService(_configuration);
        }
        #endregion

        #region PUBLIC METHODS
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> Get(int id)
        {
            #region VARIABLES
            var response = new Response();
            var request = new UsersGet();
            request.Id = id;
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                response = await _usersService.Get(request);

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

        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] UsersGetAll request)
        {
            #region VARIABLES
            var response = new Response();
            request.ItemsPage = String.IsNullOrEmpty(request.ItemsPage) ? _configuration.GetSection("AppSettings")["FiltersDefault:ItemsPage"] : request.ItemsPage;
            request.Page = String.IsNullOrEmpty(request.Page) ? _configuration.GetSection("AppSettings")["FiltersDefault:Page"] : request.Page;
            request.OrderBy = String.IsNullOrEmpty(request.OrderBy) ? _configuration.GetSection("AppSettings")["FiltersDefault:OrderBy"] : request.OrderBy;
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                response = await _usersService.GetAll(request);

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

        [HttpPost]
        public async Task<ActionResult<Response>> Create(UsersCreate request)
        {
            #region VARIABLES
            var response = new Response();
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            request.Author = _tokenService.DecriptToken(token)?.Id;
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                response = await _usersService.Create(request);

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

        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update(int id, UsersUpdate request)
        {
            #region VARIABLES
            var response = new Response();
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            request.Author = _tokenService.DecriptToken(token)?.Id;
            request.Id = id;
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                response = await _usersService.Update(request);

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

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete(int id)
        {
            #region VARIABLES
            Response response = new();
            var request = new UsersDelete();
            request.Id = id;
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                response = await _usersService.Delete(request);

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