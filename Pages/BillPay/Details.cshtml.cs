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

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly BillPayService _billPayService;

        public DetailsModel(BillPayService billPayService)
        {
            _billPayService = billPayService;
        }

        public SimpleBillPay.Models.BillPay BillPay { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillPay = await _billPayService.GetByIdAsync((int)id);

            if (BillPay == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
