using template.domain.Common;
using template.domain.Common.HttpHandle.Request;

namespace template.domain.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<Response> Token(AuthRequest req);
    }
}
