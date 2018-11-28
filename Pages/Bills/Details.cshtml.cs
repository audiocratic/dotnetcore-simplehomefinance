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

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public DetailsModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public BillInstance BillInstance { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillInstance = await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(b => b.BillTemplate.User)
                .Where(b => b.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (BillInstance == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
