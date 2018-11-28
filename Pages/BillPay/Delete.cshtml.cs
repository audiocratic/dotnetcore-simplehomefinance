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
    public class DeleteModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public DeleteModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SimpleBillPay.Models.BillPay BillPay { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillPay = await _context.BillPay.FirstOrDefaultAsync(m => m.ID == id);

            if (BillPay == null)
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

            BillPay = await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(b => b.ID == (int)id);

            if (BillPay != null)
            {
                BillPay.Payments.ForEach(p => _context.Payments.Remove(p));
                _context.BillPay.Remove(BillPay);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
