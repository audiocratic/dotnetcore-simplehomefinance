using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;

namespace SimpleBillPay.Pages.Expenses
{
    public class DetailsModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public DetailsModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public Expense Expense { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Expense = await _context.Expenses
                .Include(e => e.BillPay).FirstOrDefaultAsync(m => m.ID == id);

            if (Expense == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
