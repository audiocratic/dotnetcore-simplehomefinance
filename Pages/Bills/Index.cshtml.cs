using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Areas.Identity.Data;

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class IndexModel : BillPageModel
    {
        public int PageNumber { get; set; }
        public int TotalBills { get; set; }
        
        public IndexModel(SimpleBillPay.BudgetContext context) : base(context)
        {

        }

        public IList<BillInstance> BillInstance { get; set; }

        public async Task OnGetAsync(int? pageNumber)
        {

            //Check if pageNumber was not supplied or is less than zero
            pageNumber = pageNumber ?? 1;
            if(pageNumber < 1) pageNumber = 1;
            PageNumber = (int)pageNumber;

            //Get current user
            string userName = HttpContext.User.Identity.Name;
            SimpleBillPay.Areas.Identity.Data.User user = 
                _context.AspNetUsers
                    .Where( u => (u.UserName == userName)).FirstOrDefault();

            TotalBills = _context.BillInstance
                .Include(b => b.BillTemplate)
                .Where(b => 
                        b.BillTemplate.User.UserName == user.UserName
                    &&  b.DueDate <= DateTime.Today.AddYears(5)
                )
                .Count();

            BillInstance = await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Where(b => 
                        b.BillTemplate.User.UserName == user.UserName
                    &&  b.DueDate <= DateTime.Today.AddYears(5))
                .OrderBy(b => b.DueDate)
                .Skip((PageNumber - 1) * 50).Take(50)
                .ToListAsync();
        }
    }
}
