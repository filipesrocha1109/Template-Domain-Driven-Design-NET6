using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.RegularExpressions;
using template.crosscutting.Utils;
using template.domain.Common;
using template.domain.Common.HttpHandle.Request;
using template.domain.DTOs.Users;
using template.domain.Entities;
using template.domain.Enums;
using template.domain.Interfaces.UnitOfWork;
using template.domain.Interfaces.Users;

namespace template.services.Users
{
    public class UsersService : IUsersService
    {
        #region FIELDS
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region CTOR
        public UsersService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        #region PUBLIC METHODS
        public async Task<Response> Get(UsersGet req)
        {
            #region VARIABLES
            var userDTO = new UserDTO();
            var response = new Response();
            #endregion

            #region VALIDATIONS
            if (String.IsNullOrEmpty(req.Id.ToString()))
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
                    .Where(r => r.Id.Equals(req.Id))
                    .FirstOrDefaultAsync();

                if (user == null)
                    return response = new()
                    {
                        Message = Utils.GetEnumDescription(ReturnCodes.NotFound),
                        Code = (int)ReturnCodes.NotFound,
                        Success = false,
                        Status = (int)ReturnStatus.NotFound
                    };

                userDTO = _mapper.Map<UserDTO>(user);
                userDTO.RoleName = user.Role?.Name;
                userDTO.Name = user.Person?.Name;
                userDTO.Email = user.Person?.Email;
                userDTO.Phone = user.Person?.Phone;
                userDTO.Address = user.Person?.Address;
                userDTO.Number = user.Person?.Number;
                userDTO.District = user.Person?.District;
                userDTO.City = user.Person?.City;
                userDTO.Country = user.Person?.Country;
                userDTO.Password = null;
                userDTO.CreatedAt = Utils.ConvertDate(user.CreatedAt.ToString());
                userDTO.CreatedBy = user.CreatedBy;
                userDTO.CreatedByName = await GetAuthorName(user.CreatedBy);
                userDTO.UpdatedAt = Utils.ConvertDate(user.UpdatedAt.ToString());
                userDTO.UpdatedBy = user.UpdatedBy;
                userDTO.UpdatedByName = await GetAuthorName(user.UpdatedBy);

                return response = new()
                {
                    Data = userDTO,
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
        public async Task<Response> GetAll(UsersGetAll req)
        {
            #region VARIABLES
            var response = new Response();
            var usersDTO = new List<UserDTO>();
            int itemsPage = Convert.ToInt32(req.ItemsPage);
            int page = Convert.ToInt32(req.Page);
            int itemCount;
            //int pageCount;
            //int skipItens;
            #endregion

            #region VALIDATIONS
            #endregion

            #region LOGIC
            try
            {
                var query = _unitOfWork.UsersRepository.QueryableObject()
                    .Join(
                        _unitOfWork.RolesRepository.QueryableObject(),
                        user => user.Role.Id,
                        role => role.Id,
                        (user, role) => new
                        {
                            User = user,
                            Role = role
                        })
                    .Join(
                        _unitOfWork.PersonsRepository.QueryableObject(),
                        obj => obj.User.Person.Id,
                        person => person.Id,
                        (obj, person) => new
                        {
                            User = obj.User,
                            Role = obj.Role,
                            Person = person
                        });

                #region FILTERS
                if (!String.IsNullOrEmpty(req.Role))
                    query = query.Where(e => e.Role.Name.Equals(req.Role));

                if(req.IsActive != null)
                    query = query.Where(e => e.User.IsActive == req.IsActive);

                if (!String.IsNullOrEmpty(req.FilterOptions) &&
                    !String.IsNullOrEmpty(req.FilterText))
                {
                    switch (req.FilterOptions.ToLower())
                    {
                        case "id":
                            query = query.Where(e => e.User.Id == int.Parse(req.FilterText));
                            break;
                        case "username":
                            query = query.Where(e => e.User.Username.Contains(req.FilterText));
                            break;
                        case "name":
                            query = query.Where(e => e.Person.Name.Contains(req.FilterText));
                            break;
                        case "email":
                            query = query.Where(e => e.Person.Email.Contains(req.FilterText));
                            break;
                    }
                }
                #endregion

                #region ORDER
                if (!String.IsNullOrEmpty(req.OrderBy))
                {
                    switch (req.OrderBy.ToLower())
                    {
                        case "id asc":
                            query = query.OrderBy(e => e.User.Id);
                            break;
                        case "id desc":
                            query = query.OrderByDescending(e => e.User.Id);
                            break;
                        case "username asc":
                            query = query.OrderBy(e => e.User.Username);
                            break;
                        case "username desc":
                            query = query.OrderByDescending(e => e.User.Username);
                            break;
                        case "name asc":
                            query = query.OrderBy(e => e.Person.Name);
                            break;
                        case "name desc":
                            query = query.OrderByDescending(e => e.Person.Name);
                            break;
                        case "email asc":
                            query = query.OrderBy(e => e.Person.Email);
                            break;
                        case "email desc":
                            query = query.OrderByDescending(e => e.Person.Email);
                            break;
                    }
                }
                #endregion

                itemCount = query.Count();

                if (itemCount > 0)
                {
                    //#region PAGINATION_>=_2012_ 
                    //pageCount = (int)Math.Ceiling((decimal)itemCount / itemsPage);
                    //skipItens = (page > pageCount ? pageCount - 1 : page - 1) * itemsPage;
                    //query = query.Skip(skipItens).Take(itemsPage);
                    //#endregion

                    var usersQuery = await query
                        .Select(r =>
                            new UserDTO
                            {
                                Id = r.User.Id,
                                IsActive = r.User.IsActive,
                                PersonId = r.User.PersonId,
                                RoleId = r.User.RoleId,
                                RoleName = r.Role == null ? null : r.Role.Name,
                                Username = r.User.Username,
                                Password = null,
                                Name = r.Person == null ? null : r.Person.Name,
                                Email = r.Person == null ? null : r.Person.Email,
                                Phone = r.Person == null ? null : r.Person.Phone,
                                Address = r.Person == null ? null : r.Person.Address,
                                Number = r.Person == null ? null : r.Person.Number,
                                District = r.Person == null ? null : r.Person.District,
                                City = r.Person == null ? null : r.Person.City,
                                Country = r.Person == null ? null : r.Person.Country,
                                CreatedAt = r.User.CreatedAt.ToString(),
                                CreatedBy = r.User.CreatedBy,
                                UpdatedAt = r.User.UpdatedAt.ToString(),
                                UpdatedBy = r.User.UpdatedBy
                            })
                        .ToListAsync();

                    #region PAGINATION_<_2012_ 
                    var listUsers = new List<List<UserDTO>>();
                    int countIds = usersQuery.Count();
                    var nuberList = countIds / itemsPage;

                    for (var i = 0; i <= nuberList; i++)
                    {
                        var first = i * itemsPage;
                        var range = new List<UserDTO>();

                        if (i != nuberList)
                            range = usersQuery.GetRange(first, itemsPage);
                        else
                            range = usersQuery.GetRange(first, countIds - (i * itemsPage));

                        listUsers.Add(range);
                    }

                    if (listUsers.Count > page - 1)
                    {
                        usersDTO = listUsers[page - 1];

                        foreach (var user in usersDTO)
                        {
                            user.CreatedAt = Utils.ConvertDate(user.CreatedAt);
                            user.CreatedByName = await GetAuthorName(user.CreatedBy);
                            user.UpdatedAt = Utils.ConvertDate(user.UpdatedAt);
                            user.UpdatedByName = await GetAuthorName(user.UpdatedBy);
                        }
                    }
                    #endregion
                }

                return response = new()
                {
                    Data = usersDTO,
                    Message = Utils.GetEnumDescription(ReturnCodes.Ok),
                    Code = (int)ReturnCodes.Ok,
                    Success = true,
                    Status = (int)ReturnStatus.Ok
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.ToString());
            }
            #endregion
        }
        public async Task<Response> Create(UsersCreate req)
        {
            #region VARIABLES
            var response = new Response();
            req.IsActive = String.IsNullOrEmpty(req.IsActive.ToString()) ? false : true;
            req.Password = Utils.Encrypt(req.Password);
            #endregion

            #region VALIDATIONS
            if (String.IsNullOrEmpty(req.Username) ||
                String.IsNullOrEmpty(req.Email))
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
                var usersExist = _unitOfWork.UsersRepository.QueryableObject()
                    .Include(e => e.Person)
                    .Any(r => r.Username.Equals(req.Username) || r.Person.Email.Equals(req.Email));

                if (usersExist)
                    return response = new()
                    {
                        Message = Utils.GetEnumDescription(ReturnCodes.UserExist),
                        Code = (int)ReturnCodes.UserExist,
                        Success = false,
                        Status = (int)ReturnStatus.NotAcceptable
                    };

                var person = new Person()
                {
                    Name = req.Name,
                    Email = req.Email,
                    Phone = req.Phone,
                    Address = req.Address,
                    Number = req.Number,
                    City = req.City,
                    District = req.District,
                    Country = req.Country,
                    CreatedAt = DateTime.Now
                };

                _unitOfWork.PersonsRepository.Insert(person);
                await _unitOfWork.PersistchangesAsync();

                var user = new User()
                {
                    IsActive = req.IsActive,
                    Username = req.Username,
                    Password = req.Password,
                    RoleId = req.RoleId,
                    PersonId = person.Id,
                    CreatedAt = DateTime.Now,
                    CreatedBy = req.Author
                };

                _unitOfWork.UsersRepository.Insert(user);
                await _unitOfWork.PersistchangesAsync();

                UserDTO userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Password = null;
                userDTO.CreatedAt = Utils.ConvertDate(user.CreatedAt.ToString());
                userDTO.CreatedBy = user.CreatedBy;
                userDTO.CreatedByName = await GetAuthorName(user.CreatedBy);

                return response = new()
                {
                    Data = userDTO,
                    Message = Utils.GetEnumDescription(ReturnCodes.Ok),
                    Code = (int)ReturnCodes.Ok,
                    Success = true,
                    Status = (int)ReturnStatus.Ok
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.ToString());
            }
            #endregion
        }
        public async Task<Response> Update(UsersUpdate req)
        {
            #region VARIABLES
            var response = new Response();
            #endregion

            #region VALIDATIONS
            if (String.IsNullOrEmpty(req.Id.ToString()))
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
                    .Where(r => r.Id.Equals(req.Id))
                    .FirstOrDefaultAsync();

                if (user == null)
                    return response = new()
                    {
                        Message = Utils.GetEnumDescription(ReturnCodes.NotFound),
                        Code = (int)ReturnCodes.NotFound,
                        Success = false,
                        Status = (int)ReturnStatus.NotFound
                    };

                user.IsActive = req.IsActive != null ? req.IsActive : user.IsActive;
                user.Username = !String.IsNullOrEmpty(req.Username) ? req.Username : user.Username;
                user.Password = !String.IsNullOrEmpty(req.Password) ? Utils.Encrypt(req.Password) : user.Password;
                user.RoleId = req.RoleId != null ? (int)req.RoleId : user.RoleId;
                user.UpdatedBy = req.Author;
                user.UpdatedAt = DateTime.Now;

                if (user.Person != null)
                {
                    user.Person.Name = !String.IsNullOrEmpty(req.Name) ? req.Name : user.Person.Name;
                    user.Person.Email = !String.IsNullOrEmpty(req.Email) ? req.Email : user.Person.Email;
                    user.Person.Phone = !String.IsNullOrEmpty(req.Phone) ? req.Phone : user.Person.Phone;
                    user.Person.Address = !String.IsNullOrEmpty(req.Address) ? req.Address : user.Person.Address;
                    user.Person.Number = !String.IsNullOrEmpty(req.Number) ? req.Number : user.Person.Number;
                    user.Person.District = !String.IsNullOrEmpty(req.District) ? req.District : user.Person.District;
                    user.Person.City = !String.IsNullOrEmpty(req.City) ? req.City : user.Person.City;
                    user.Person.Country = !String.IsNullOrEmpty(req.Country) ? req.Country : user.Person.Country;

                    _unitOfWork.PersonsRepository.Update(user.Person);
                }

                _unitOfWork.UsersRepository.Update(user);
                await _unitOfWork.PersistchangesAsync();

                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.RoleName = user.Role?.Name;
                userDTO.Name = user.Person?.Name;
                userDTO.Email = user.Person?.Email;
                userDTO.Phone = user.Person?.Phone;
                userDTO.Address = user.Person?.Address;
                userDTO.Number = user.Person?.Number;
                userDTO.District = user.Person?.District;
                userDTO.City = user.Person?.City;
                userDTO.Country = user.Person?.Country;
                userDTO.Password = null;
                userDTO.CreatedAt = Utils.ConvertDate(user.CreatedAt.ToString());
                userDTO.CreatedBy = user.CreatedBy;
                userDTO.CreatedByName = await GetAuthorName(user.CreatedBy);
                userDTO.UpdatedAt = Utils.ConvertDate(user.UpdatedAt.ToString());
                userDTO.UpdatedBy = user.UpdatedBy;
                userDTO.UpdatedByName = await GetAuthorName(user.UpdatedBy);

                return response = new()
                {
                    Data = userDTO,
                    Message = Utils.GetEnumDescription(ReturnCodes.Ok),
                    Code = (int)ReturnCodes.Ok,
                    Success = true,
                    Status = (int)ReturnStatus.Ok
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.ToString());
            }
            #endregion
        }
        public async Task<Response> Delete(UsersDelete req)
        {
            #region VARIABLES
            var response = new Response();
            var userDTO = new UserDTO();
            #endregion

            #region VALIDATIONS
            if (String.IsNullOrEmpty(req.Id.ToString()))
                return null;
            #endregion

            #region LOGIC
            try
            {
                var user = _unitOfWork.UsersRepository.Get(r => r.Id == req.Id);

                if (user == null)
                    return response = new()
                    {
                        Message = Utils.GetEnumDescription(ReturnCodes.NotFound),
                        Code = (int)ReturnCodes.NotFound,
                        Success = false,
                        Status = (int)ReturnStatus.NotFound
                    };

                var person = _unitOfWork.PersonsRepository.Get(r => r.Id == user.PersonId);

                if (person != null)
                    _unitOfWork.PersonsRepository.Delete(person);

                _unitOfWork.UsersRepository.Delete(user);
                await _unitOfWork.PersistchangesAsync();

                userDTO.Id = req.Id;

                return response = new()
                {
                    Data = userDTO,
                    Message = Utils.GetEnumDescription(ReturnCodes.Ok),
                    Code = (int)ReturnCodes.Ok,
                    Success = true,
                    Status = (int)ReturnStatus.Ok
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.ToString());
            }
            #endregion
        }
        public async Task<string?> GetAuthorName(int? id)
        {
            #region VARIABLES
            #endregion

            #region VALIDATIONS
            if (id <= 0 || id == null)
                return null;
            #endregion 

            #region LOGIC
            try
            {
                var user = await _unitOfWork.UsersRepository.QueryableObject()
                    .Include(e => e.Person)
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
               
                if (user == null)
                    return null;

                return user.Person?.Name;
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