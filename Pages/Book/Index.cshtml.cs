using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorBookStore.DAL;
using RazorBookStore.Models;

namespace RazorBookStore.Pages.Book
{
    public class IndexModel : PageModel
    {
        private readonly MyAppDbContext _dbContext;
        public IndexModel(MyAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Books> Books { get; set; }

        public async Task OnGetAsync()
        {
            Books = await _dbContext.Books.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var book = await _dbContext.Books.FindAsync(id);
            if (book == null) {
                return NotFound();
            }
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
