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
        public Users User { get; set; } = default!;
        
        public IList<RoleEntity> AvailableRoles { get; set; } = new List<RoleEntity>();
        
        [BindProperty]
        public long? SelectedRole { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            System.Diagnostics.Debug.WriteLine($"=== Edit User GET Start ===");
            System.Diagnostics.Debug.WriteLine($"Requested ID: {id}");
            
            if (id == null)
            {
                System.Diagnostics.Debug.WriteLine("ID is null - this might be a POST redirecting to GET");
                return RedirectToPage("./Index");
            }
            
            User = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (User == null)
            {
                System.Diagnostics.Debug.WriteLine("User not found!");
                return NotFound();
            }
            
            System.Diagnostics.Debug.WriteLine($"Found User: {User.Username}, ID: {User.Id}");
            
            // Load available roles
            AvailableRoles = await _dbContext.Roles.ToListAsync();
            System.Diagnostics.Debug.WriteLine($"AvailableRoles count: {AvailableRoles.Count}");
            
            // Set current selected role
            var currentRole = User.UserRoles?.FirstOrDefault();
            SelectedRole = currentRole?.RoleId;
            System.Diagnostics.Debug.WriteLine($"Current SelectedRole: {SelectedRole}");

            // Don't send password hash to client for security
            User.PasswordHash = string.Empty;

            System.Diagnostics.Debug.WriteLine("=== Edit User GET End ===");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            System.Diagnostics.Debug.WriteLine($"=== Edit User POST Start ===");
            System.Diagnostics.Debug.WriteLine($"User.Id: {User.Id}");
            System.Diagnostics.Debug.WriteLine($"User.Username: {User.Username}");
            System.Diagnostics.Debug.WriteLine($"SelectedRole: {SelectedRole}");
            System.Diagnostics.Debug.WriteLine($"User.PasswordHash: '{User.PasswordHash}'");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            
            // Remove PasswordHash validation if empty (user doesn't want to change password)
            if (string.IsNullOrEmpty(User.PasswordHash))
            {
                ModelState.Remove("User.PasswordHash");
                System.Diagnostics.Debug.WriteLine("Removed PasswordHash from validation");
            }
            
            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("ModelState is INVALID");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Field: {state.Key}");
                        foreach (var error in state.Value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  Error: {error.ErrorMessage}");
                        }
                    }
                }
                AvailableRoles = await _dbContext.Roles.ToListAsync();
                return Page();
            }

            try
            {
                // Update UserRoles first
                var existingUserRoles = await _dbContext.UserRoles
                    .Where(ur => ur.UserId == User.Id)
                    .ToListAsync();
                
                System.Diagnostics.Debug.WriteLine($"Existing UserRoles count: {existingUserRoles.Count}");
                
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
                    System.Diagnostics.Debug.WriteLine($"Added new UserRole: UserId={User.Id}, RoleId={SelectedRole.Value}");
                }
                
                // Get original user to preserve password if not changed
                var originalUser = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == User.Id);
                if (originalUser == null)
                {
                    return NotFound();
                }
                
                // Update User - preserve password if empty
                if (string.IsNullOrEmpty(User.PasswordHash))
                {
                    User.PasswordHash = originalUser.PasswordHash;
                    System.Diagnostics.Debug.WriteLine("Kept original password");
                }
                else
                {
                    // Hash new password before saving
                    User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(User.PasswordHash);
                    System.Diagnostics.Debug.WriteLine("Hashed new password");
                }
                
                _dbContext.Attach(User).State = EntityState.Modified;
                
                var changes = await _dbContext.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"SaveChangesAsync returned: {changes}");
                
                System.Diagnostics.Debug.WriteLine("=== Edit User POST Success ===");
            }
            catch (DbUpdateConcurrencyException)
            {
                System.Diagnostics.Debug.WriteLine("DbUpdateConcurrencyException caught");
                if (!_dbContext.Users.Any(e => e.Id == User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error updating user: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
                AvailableRoles = await _dbContext.Roles.ToListAsync();
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
