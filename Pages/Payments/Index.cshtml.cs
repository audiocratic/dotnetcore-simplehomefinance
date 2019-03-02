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
    public class IndexModel : PageModel
    {
        private readonly PaymentService _paymentService;

        public IndexModel(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public IList<Payment> Payment { get;set; }

        public async Task OnGetAsync()
        {
            Payment = await _paymentService.GetPaymentsAsync();
        }
    }
}
