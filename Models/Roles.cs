using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RoleEntity
{
    public long Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
}
