using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;

namespace SimpleBillPay.Pages.Bills
{
    public class EditModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public EditModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BillInstance BillInstance { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillInstance = await _context.BillInstance
                .Include(b => b.BillTemplate).FirstOrDefaultAsync(m => m.ID == id);

            if (BillInstance == null)
            {
                return NotFound();
            }
           ViewData["BillTemplateID"] = new SelectList(_context.BillTemplate, "ID", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(BillInstance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillInstanceExists(BillInstance.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BillInstanceExists(int id)
        {
            return _context.BillInstance.Any(e => e.ID == id);
        }
    }
}
