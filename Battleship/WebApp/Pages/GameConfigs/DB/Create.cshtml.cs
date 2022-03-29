using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameConfig = Domain.GameConfig;
using ShipConfig = Domain.ShipConfig;

namespace WebAppTest.Pages.GameConfigs.DB
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public GameConfig GameConfig { get; set; }  = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            GameConfig.ShipConfigs = new List<ShipConfig>();
            GameConfig.SavedGames = new List<SavedGame>();
            GameConfig.GameConfigName = GameConfig.GameConfigName.Replace(" ", "_");

            _context.GameConfigs.Add(GameConfig);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
