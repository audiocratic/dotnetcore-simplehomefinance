using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Services;
using SimpleBillPay.Areas.Identity.Data;

namespace SimpleBillPay.Pages.Payments
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly PaymentService _paymentService;
        private readonly UserManager<User> _userManager;

        public CreateModel(PaymentService paymentService, UserManager<User> userManager)
        {
            _paymentService = paymentService;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
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

            Payment.BillInstance.BillTemplate.User = await _userManager.GetUserAsync(HttpContext.User);

            await _paymentService.AddAsync(Payment);

            return RedirectToPage("./Index");
        }
    }
}