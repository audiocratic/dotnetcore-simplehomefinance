using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleBillPay;
using SimpleBillPay.Models;

namespace SimpleBillPay.Pages.Payments
{
    public class CreateModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public CreateModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["BillInstanceID"] = new SelectList(_context.BillInstance, "ID", "Name");
        ViewData["BillPayID"] = new SelectList(_context.BillPay, "ID", "UserId");
            return Page();
        }

        [BindProperty]
        public Payment Payment { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Payments.Add(Payment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}