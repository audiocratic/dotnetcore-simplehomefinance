using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;

namespace SimpleBillPay.Pages.Payments
{
    public class DeleteModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public DeleteModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Payment Payment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Payment = await _context.Payments
                .Include(p => p.BillInstance)
                .Include(p => p.BillPay).FirstOrDefaultAsync(m => m.ID == id);

            if (Payment == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Payment = await _context.Payments.FindAsync(id);

            if (Payment != null)
            {
                _context.Payments.Remove(Payment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
