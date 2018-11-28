using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public DetailsModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public SimpleBillPay.Models.BillPay BillPay { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillPay = await _context.BillPay
                .Include(b => b.User)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (BillPay == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
