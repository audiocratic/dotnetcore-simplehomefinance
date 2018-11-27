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
using SimpleBillPay.Pages.BillPay;

namespace SimpleBillPay.Pages.Payments
{
    public class EditModel : BillPayPageModel
    {

        public EditModel(SimpleBillPay.BudgetContext context) : base(context) {}

        [BindProperty]
        public Payment Payment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Payment = await GetPaymentById((int)id);

            if (Payment == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnBillPayID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Payment payment = await GetPaymentById(Payment.ID);

            if(payment == null) return NotFound();

            _context.Attach(payment).State = EntityState.Modified;

            payment.Amount = Payment.Amount;
            payment.DateConfirmed = Payment.DateConfirmed;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(Payment.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if(returnBillPayID != null)
            {
                int billPayID;
                
                if(int.TryParse(returnBillPayID, out billPayID))
                {
                    return RedirectToPage("/BillPay/Edit", new {id = returnBillPayID});
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.ID == id);
        }
    }
}
