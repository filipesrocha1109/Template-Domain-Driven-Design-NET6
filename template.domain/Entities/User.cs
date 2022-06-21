using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace template.domain.Entities
{
    public class User
    {
        [Key, Required]
        public int Id { get; set; }
        public bool? IsActive { get; set; }
        [ForeignKey("Role")]
        public int? RoleId { get; set; }
        [ForeignKey("Person")]
        public int? PersonId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public virtual Person? Person { get; set; }
        public virtual Role? Role { get; set; }

    }
}
