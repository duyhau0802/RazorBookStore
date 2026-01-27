using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorBookStore.DAL;
using RazorBookStore.Models;

namespace RazorBookStore.Pages.Book
{
    public class EditModel : PageModel
    {
        private readonly MyAppDbContext _dbContext;

        public EditModel(MyAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty] 
        public Books Books { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return RedirectToPage("./Index");
            }
            var book = await _dbContext.Books.FindAsync(id);
            if(book == null)
            {
                return RedirectToPage("./Index");
            }
            Books = book;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            var bookInDb = await _dbContext.Books.FindAsync(Books.Id);
            if(bookInDb == null)
            {
                return RedirectToPage("./Index");
            }
            bookInDb.Title = Books.Title;
            bookInDb.Descripton = Books.Descripton;
            bookInDb.Author = Books.Author;
            bookInDb.Price = Books.Price;
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
