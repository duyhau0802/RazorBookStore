using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorBookStore.Models
{
    public class UserRoles
    {
        [Key]
        [Column(Order = 1)]
        public long UserId { get; set; }
        
        [Key]
        [Column(Order = 2)]
        public long RoleId { get; set; }

        public Users User { get; set; }
        public RoleEntity Role { get; set; }
    }
}
