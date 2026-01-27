using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorBookStore.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }
        [MaxLength(255)]
        public string FullName { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Users>()
                .HasIndex(u => u.Username)
                .IsUnique();
            builder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
