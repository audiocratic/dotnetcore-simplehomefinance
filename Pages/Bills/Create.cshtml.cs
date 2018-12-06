using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Areas.Identity.Data;

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class CreateModel : BillPageModel
    {

        public CreateModel(SimpleBillPay.BudgetContext context) : base(context)
        {
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

            //Get current user
            string userName = HttpContext.User.Identity.Name;
            SimpleBillPay.Areas.Identity.Data.User user = 
                _context.AspNetUsers
                    .Where( u => (u.UserName == userName)).FirstOrDefault();

            //Add user to this template
            BillInstance.BillTemplate.User = user;

            //Bill instance amount should match template amount
            BillInstance.BillTemplate.Amount = BillInstance.Amount;
            BillInstance.Name = BillInstance.BillTemplate.Name;

            _context.BillInstance.Add(BillInstance);

            if(BillInstance.BillTemplate.FrequencyInMonths > 0)
            {
                CreateSeries(BillInstance);
            }
            
            //Insert
            await _context.SaveChangesAsync();

            if(returnBillPayID != null && returnBillPayID > 0)
            {
                return RedirectToPage("/BillPay/Edit", new {id = returnBillPayID});
            }

            return RedirectToPage("./Index");
        }
    }
}