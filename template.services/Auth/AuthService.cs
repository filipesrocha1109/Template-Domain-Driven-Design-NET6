using AutoMapper;
using Microsoft.EntityFrameworkCore;
using template.crosscutting.Utils;
using template.domain.Common;
using template.domain.Common.HttpHandle.Request;
using template.domain.DTOs.Users;
using template.domain.Enums;
using template.domain.Interfaces.Auth;
using template.domain.Interfaces.UnitOfWork;

namespace template.services.Auth
{
    public class AuthService : IAuthService
    {
        #region FIELDS
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region CTOR
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        #region PUBLIC METHODS
        public async Task<Response> Token(AuthRequest req)
        {
            #region VARIABLES
            var response = new Response();
            var userAuthDTO = new UserAuthDTO();
            req.Password = Utils.Encrypt(req.Password);
            #endregion

            #region VALIDATIONS
            if (String.IsNullOrEmpty(req.Username) ||
                String.IsNullOrEmpty(req.Password))
                return response = new()
                {
                    Message = Utils.GetEnumDescription(ReturnCodes.InvalidValue),
                    Code = (int)ReturnCodes.InvalidValue,
                    Success = false,
                    Status = (int)ReturnStatus.BadRequest
                };
            #endregion 

            #region LOGIC
            try
            {
                var user = await _unitOfWork.UsersRepository.QueryableObject()
                    .Include(e => e.Person)
                    .Include(e => e.Role)
                    .Where(r => r.IsActive == true && 
                        r.Username.Equals(req.Username) && 
                        r.Password.Equals(req.Password))
                    .FirstOrDefaultAsync();


                if(user == null)
                    return response = new()
                    {
                        Message = Utils.GetEnumDescription(ReturnCodes.NotFound),
                        Code = (int)ReturnCodes.NotFound,
                        Success = false,
                        Status = (int)ReturnStatus.NotFound
                    };

                userAuthDTO = new UserAuthDTO()
                {
                    Id = user.Id,
                    IsActive = user.IsActive,
                    Username = user.Username,
                    Name = user.Person?.Name,
                    Role = user.Role?.Name
                };

                return response = new()
                {
                    Data = userAuthDTO,
                    Message = Utils.GetEnumDescription(ReturnCodes.Ok),
                    Code = (int)ReturnCodes.Ok,
                    Success = true,
                    Status = (int)ReturnStatus.Ok
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            #endregion  
        }
        #endregion

        #region PRIVATE METHODS
        #endregion
    }
}
