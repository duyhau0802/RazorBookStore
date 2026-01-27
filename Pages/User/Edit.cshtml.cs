using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorBookStore.DAL;
using RazorBookStore.Models;

namespace RazorBookStore.Pages.User
{
    public class EditModel : PageModel
    {
        private readonly MyAppDbContext _dbContext;

        public EditModel(MyAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public Users User { get; set; }
        
        public IList<RoleEntity> AvailableRoles { get; set; } = new List<RoleEntity>();
        
        [BindProperty]
        public long? SelectedRole { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            User = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (User == null)
            {
                return NotFound();
            }
            
            // Load available roles
            AvailableRoles = await _dbContext.Roles.ToListAsync();
            
            // Set current selected role
            var currentRole = User.UserRoles?.FirstOrDefault();
            SelectedRole = currentRole?.RoleId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableRoles = await _dbContext.Roles.ToListAsync();
                return Page();
            }

            _dbContext.Attach(User).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                
                // Update UserRoles
                var existingUserRoles = await _dbContext.UserRoles
                    .Where(ur => ur.UserId == User.Id)
                    .ToListAsync();
                
                // Remove existing roles
                _dbContext.UserRoles.RemoveRange(existingUserRoles);
                
                // Add new role if selected
                if (SelectedRole.HasValue && SelectedRole.Value > 0)
                {
                    var newUserRole = new UserRoles
                    {
                        UserId = User.Id,
                        RoleId = SelectedRole.Value
                    };
                    _dbContext.UserRoles.Add(newUserRole);
                }
                
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Users.Any(e => e.Id == User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
