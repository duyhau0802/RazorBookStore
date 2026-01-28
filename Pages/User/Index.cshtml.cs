using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBookStore.DAL;
using RazorBookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace RazorBookStore.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly MyAppDbContext _dbContext;

        public IndexModel(MyAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Users> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
            
            // Debug: Check if UserRoles are loaded
            foreach (var user in Users)
            {
                System.Diagnostics.Debug.WriteLine($"User: {user.Username}, UserRoles count: {user.UserRoles?.Count ?? 0}");
                foreach (var userRole in user.UserRoles ?? new List<UserRoles>())
                {
                    System.Diagnostics.Debug.WriteLine($"  - Role: {userRole.Role?.Name}");
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Remove UserRoles first (due to foreign key constraint)
            _dbContext.UserRoles.RemoveRange(user.UserRoles);
            
            // Remove User
            _dbContext.Users.Remove(user);
            
            await _dbContext.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }

    }
}
