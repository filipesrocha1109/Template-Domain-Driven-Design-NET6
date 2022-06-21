
using System.ComponentModel.DataAnnotations;

namespace template.domain.Common.HttpHandle.Request
{
    public class AuthRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }

    public class HttpClientParams
    {
        public string? param { get; set; }
        public string? value { get; set; }
    }

    public class UsersGet
    {
        public int? Id { get; set; }
    }

    public class UsersGetAll
    {
        public string? ItemsPage { get; set; }
        public string? Page { get; set; }
        public string? Role { get; set; }
        public string? FilterOptions { get; set; }/// Id, Usuario, Nome, E-mail
        public string? FilterText { get; set; }
        public string? OrderBy { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UsersCreate
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Number { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? Author { get; set; }

    }

    public class UsersUpdate
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Number { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? Author { get; set; }
    }

    public class UsersDelete
    {
        public int? Id { get; set; }
    }
}
