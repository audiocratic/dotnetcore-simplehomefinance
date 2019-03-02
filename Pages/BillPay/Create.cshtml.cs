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
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using SimpleBillPay.Areas.Identity.Data;
using SimpleBillPay.Services;


namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly BillPayService _billPayService;

        public CreateModel (UserManager<User> userManager, BillPayService billPayService)
        {
            _userManager = userManager;
            _billPayService = billPayService;
        }

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

            BillPay.User = await _userManager.GetUserAsync(HttpContext.User);

            try
            {
                await _billPayService.AddAsync(BillPay);
            }
            catch(DbUpdateException ex)
            {
                if(ex.InnerException is MySqlException)
                {
                    if(((MySqlException)ex.InnerException).Number == 1062)
                    {
                        return RedirectToPage("./Create", 
                            new 
                            {
                                id = BillPay.ID, 
                                duplicateDateError = BillPay.BillPayDate.ToString("yyyy-MM-dd")
                            });
                    }
                }
            }


            return RedirectToPage("./Edit", new {id = BillPay.ID});
        }
    }
}