using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebAppTest.Pages_SavedGames
{
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<SavedGame> SavedGame { get;set; } = default!;

        public async Task OnGetAsync()
        {
            SavedGame = await _context.SavedGames
                .Include(s => s.GameConfig).ToListAsync();
        }
    }
}
