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
using SimpleBillPay.Services;

namespace SimpleBillPay.Pages.Payments
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly PaymentService _paymentService;

        public DeleteModel(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [BindProperty]
        public Payment Payment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Payment = await _paymentService.GetPaymentByIdAsync((int)id);

            if (Payment != null)
            {
                await _paymentService.RemoveAsync(Payment);
            }

            return RedirectToPage("./Index");
        }
    }
}
