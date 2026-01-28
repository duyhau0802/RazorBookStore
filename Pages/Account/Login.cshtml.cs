using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBookStore.DAL;
using RazorBookStore.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RazorBookStore.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MyAppDbContext _context;
        
        public LoginModel(MyAppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new();

        public class LoginInputModel
        {
            [Required]
            public string Username { get; set; } = "";
            
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
            
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // If user is already logged in, redirect to home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Index");
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Find user by username
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == Input.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Create claims for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("FullName", user.FullName ?? "")
            };

            // Add role claims
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role?.Name ?? ""));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                ExpiresUtc = Input.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(1)
            };

            // Sign in the user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Index");
        }
    }
}
