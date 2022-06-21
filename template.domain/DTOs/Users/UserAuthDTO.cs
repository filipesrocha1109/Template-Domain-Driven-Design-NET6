namespace template.domain.DTOs.Users
{
    public class UserAuthDTO
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
    }
}
