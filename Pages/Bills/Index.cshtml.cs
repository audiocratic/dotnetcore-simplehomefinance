using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Services;
using SimpleBillPay.Areas.Identity.Data;

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly BillService _billService;
        
        public int PageNumber { get; set; }
        public int TotalBills { get; set; }
        
        public IndexModel(BillService billService)
        {
            _billService = billService;
        }

        public IList<BillInstance> BillInstances { get; set; }

        public async Task OnGetAsync(int? pageNumber)
        {

            //Check if pageNumber was not supplied or is less than zero
            pageNumber = pageNumber ?? 1;
            if(pageNumber < 1) pageNumber = 1;
            PageNumber = (int)pageNumber;

            TotalBills = await _billService.CountBillsOnOrBeforeDueDateAsync(DateTime.Today.AddYears(5));

            BillInstances = await _billService.GetBillsOnOrBeforeDueDateAsync(
                DateTime.Today.AddYears(5),
                (PageNumber - 1) * 50,
                50
            );
        }
    }
}
