using template.domain.Common;
using template.domain.Common.HttpHandle.Request;

namespace template.domain.Interfaces.Users
{
    public interface IUsersService
    {
        Task<Response> Get(UsersGet req);
        Task<Response> GetAll(UsersGetAll req);
        Task<Response> Create(UsersCreate req);
        Task<Response> Update(UsersUpdate req);
        Task<Response> Delete(UsersDelete req);

    }
}
