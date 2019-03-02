using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Services;
using SimpleBillPay.Pages.BillPay;

namespace SimpleBillPay.Pages.Payments
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly PaymentService _paymentService;
        public EditModel(PaymentService paymentService) 
        {
            _paymentService = paymentService;
        }

        [BindProperty]
        public Payment Payment { get; set; }

        public int? ReturnBillPayID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? returnBillPayID)
        {
            if (id == null)
            {
                return NotFound();
            }

            Payment = await _paymentService.GetPaymentByIdAsync((int)id);

            if (Payment == null)
            {
                return NotFound();
            }

            ReturnBillPayID = returnBillPayID;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? returnBillPayID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Payment payment = await _paymentService.GetPaymentByIdAsync(Payment.ID);

            if(payment == null) return NotFound();

            payment.Amount = Payment.Amount;
            payment.DateConfirmed = Payment.DateConfirmed;

            try
            {
                await _paymentService.UpdateAsync(payment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _paymentService.PaymentExistsAsync(Payment.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if(returnBillPayID != null && returnBillPayID > 0)
            {
                return RedirectToPage("/BillPay/Edit", new {id = returnBillPayID});
            }

            return RedirectToPage("./Index");
        }

        
    }
}
