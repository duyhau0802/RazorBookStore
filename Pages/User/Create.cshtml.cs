using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorBookStore.DAL;
using RazorBookStore.Models;

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
