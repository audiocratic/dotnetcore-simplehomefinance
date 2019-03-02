using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Services;
using SimpleBillPay.Areas.Identity.Data;

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly BillService _billService;
        private readonly UserManager<User> _userManager;

        public CreateModel(BillService billService, UserManager<User> userManager)
        {
            _billService = billService;
            _userManager = userManager;
        }

        public int? ReturnBillPayID { get; set; }

        public IActionResult OnGet(int? returnBillPayID)
        {
            ReturnBillPayID = returnBillPayID;
            
            return Page();
        }

        [BindProperty]
        public BillInstance BillInstance { get; set; }

        

        public async Task<IActionResult> OnPostAsync(int? returnBillPayID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Add user to this template
            BillInstance.BillTemplate.User = await _userManager.GetUserAsync(HttpContext.User);

            //Bill instance amount should match template amount
            BillInstance.BillTemplate.Amount = BillInstance.Amount;
            BillInstance.Name = BillInstance.BillTemplate.Name;

            if(BillInstance.BillTemplate.FrequencyInMonths > 0)
            {
                await _billService.CreateSeriesAsync(BillInstance);
            }
            
            //Insert
            await _billService.AddAsync(BillInstance);

            if(returnBillPayID != null && returnBillPayID > 0)
            {
                return RedirectToPage("/BillPay/Edit", new {id = returnBillPayID});
            }

            return RedirectToPage("./Index");
        }
    }
}