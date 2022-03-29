using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages.GameConfigs.File
{
    public class CreateModelFile : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public GameConfig GameConfig { get; set; }  = default!;
    }
}
