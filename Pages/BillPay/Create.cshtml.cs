using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleBillPay;
using SimpleBillPay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class CreateModel : BillPayPageModel
    {

        public CreateModel (SimpleBillPay.BudgetContext context) : base(context){}

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SimpleBillPay.Models.BillPay BillPay { get; set; }
        

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            BillPay.User = 
                _context.AspNetUsers
                .Where(u => u.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefault();

            _context.BillPay.Add(BillPay);
            await _context.SaveChangesAsync();


            return RedirectToPage("./Edit", new {id = BillPay.ID});
        }
    }
}