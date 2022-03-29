using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppTest.Pages.ShipConfigs
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public SelectList GameConfigSelectList { get; set; } = default!;

        public IActionResult OnGet()
        {
        GameConfigSelectList = new SelectList(_context.GameConfigs, nameof(GameConfig.GameConfigId), nameof(GameConfig.GameConfigName));
            return Page();
        }

        [BindProperty] public ShipConfig ShipConfig { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            _context.ShipConfigs.Add(ShipConfig);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
