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

namespace SimpleBillPay.Pages.Payments
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public DetailsModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public Payment Payment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Payment = await _context.Payments
                .Include(p => p.BillInstance)
                .Include(p => p.BillInstance.BillTemplate)
                .Include(p => p.BillInstance.BillTemplate.User)
                .Where(p => p.BillInstance.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Payment == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
