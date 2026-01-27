using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBookStore.DAL;
using RazorBookStore.Models;

namespace RazorBookStore.Pages.Book
{
    public class CreateModel : PageModel
    {
        private readonly MyAppDbContext _context;

        public CreateModel(MyAppDbContext context)
        {
            this._context = context;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        [BindProperty]
        public Books Books { get; set; } = default;

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Books.Add(Books);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

    }
}
