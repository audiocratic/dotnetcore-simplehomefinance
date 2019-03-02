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

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly BillService _billService;

        public DetailsModel(BillService billService)
        {
            _billService = billService;
        }

        public BillInstance BillInstance { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillInstance = await _billService.GetBillInstanceAsync((int)id);

            if (BillInstance == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
