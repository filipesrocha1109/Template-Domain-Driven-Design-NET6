using AutoMapper;
using template.domain.DTOs.Users;
using template.domain.Entities;

namespace template.services.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
