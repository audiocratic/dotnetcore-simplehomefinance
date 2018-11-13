using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Areas.Identity.Data;

namespace SimpleBillPay.Pages.Bills
{
    public class CreateModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public CreateModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public BillInstance BillInstance { get; set; }

        public async Task<IActionResult> OnPostAsync()
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

            _context.BillInstance.Add(BillInstance);

            if(BillInstance.BillTemplate.FrequencyInMonths > 0)
            {
                int instancesToAdd = 360 / BillInstance.BillTemplate.FrequencyInMonths;

                for(int i = 1; i <= instancesToAdd; i++)
                {
                    DateTime dueDate = BillInstance.DueDate.AddMonths(i * BillInstance.BillTemplate.FrequencyInMonths);

                    BillInstance instance = new BillInstance();

                    instance.BillTemplate = BillInstance.BillTemplate;
                    instance.DueDate = dueDate;
                    instance.Amount = BillInstance.Amount;

                    _context.BillInstance.Add(instance);
                }
            }
            
            //Insert
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}