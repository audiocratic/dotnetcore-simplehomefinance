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
    public class DeleteModel : PageModel
    {
        private readonly BillService _billService;

        public DeleteModel(BillService billService)
        {
            _billService = billService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillInstance = await _billService.GetBillInstanceAsync((int)id);

            if (BillInstance != null)
            {
                await _billService.RemoveAsync(BillInstance);
            }

            return RedirectToPage("./Index");
        }
    }
}
