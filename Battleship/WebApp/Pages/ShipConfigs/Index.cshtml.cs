using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebAppTest.Pages_ShipConfigs
{
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ShipConfig> ShipConfig { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ShipConfig = await _context.ShipConfigs
                .Include(s => s.GameConfig).ToListAsync();
        }
    }
}
