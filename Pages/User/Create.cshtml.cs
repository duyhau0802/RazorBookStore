using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBookStore.DAL;
using RazorBookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace RazorBookStore.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly MyAppDbContext _context;
        
        public CreateModel(MyAppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Users User { get; set; } = default!;
        
        public IList<RoleEntity> AvailableRoles { get; set; } = new List<RoleEntity>();
        
        [BindProperty]
        public long? SelectedRole { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AvailableRoles = await _context.Roles.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableRoles = await _context.Roles.ToListAsync();
                return Page();
            }

            // Set CreatedAt
            User.CreatedAt = DateTime.UtcNow;
            
            // Hash password before saving
            if (!string.IsNullOrEmpty(User.PasswordHash))
            {
                User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(User.PasswordHash);
            }
            
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
            
            // Add UserRole if selected
            if (SelectedRole.HasValue && SelectedRole.Value > 0)
            {
                var userRole = new UserRoles
                {
                    UserId = User.Id,
                    RoleId = SelectedRole.Value
                };
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage("./Index");
        }
    }
}
