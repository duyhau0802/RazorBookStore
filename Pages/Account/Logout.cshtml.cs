using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorBookStore.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // If user is not logged in, redirect to home
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Index");
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Clear session
            HttpContext.Session.Clear();
            
            return RedirectToPage("/Index");
        }
    }
}
