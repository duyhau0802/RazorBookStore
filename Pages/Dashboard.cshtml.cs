using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace RazorBookStore.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IMemoryCache _cache;
        
        public DashboardModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string UserId { get; set; } = "";
        public DateTime LoginTime { get; set; }
        public string SessionId { get; set; } = "";
        public string CacheValue { get; set; } = "";
        public bool IsAuthenticated { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            IsAuthenticated = User.Identity?.IsAuthenticated == true;
            
            if (IsAuthenticated)
            {
                // Get user ID from claims
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
                
                // Set session data
                LoginTime = DateTime.Now;
                SessionId = HttpContext.Session.Id;
                
                // Store login time in session
                HttpContext.Session.SetString("LoginTime", LoginTime.ToString());
                HttpContext.Session.SetString("Username", User.Identity?.Name ?? "");
                
                // Use cache
                if (!_cache.TryGetValue("DashboardVisitCount", out int visitCount))
                {
                    visitCount = 0;
                }
                visitCount++;
                
                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                
                _cache.Set("DashboardVisitCount", visitCount, cacheEntryOptions);
                CacheValue = $"Dashboard visited {visitCount} times";
            }
            else
            {
                // For non-authenticated users
                SessionId = HttpContext.Session.Id;
                CacheValue = "Please login to see personalized data";
            }
            
            return Page();
        }
        
        // Example of using cache in POST
        public async Task<IActionResult> OnPostClearCacheAsync()
        {
            _cache.Remove("DashboardVisitCount");
            return RedirectToPage();
        }
    }
}
