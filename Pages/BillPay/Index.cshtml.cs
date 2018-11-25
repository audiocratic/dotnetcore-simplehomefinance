using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using Microsoft.AspNetCore.Authorization;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public IndexModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public ListDirection Direction { get; set; }

        public enum ListDirection
        {
            RETRO, FUTURE
        }

        public IList<SimpleBillPay.Models.BillPay> BillPays { get;set; }

        public async Task OnGetAsync(string direction)
        {
            if(direction == null || direction.ToLower() == ListDirection.FUTURE.ToString().ToLower())
            {
                Direction = ListDirection.FUTURE;
            }
            else
            {
                Direction = ListDirection.RETRO;
            }

            switch(Direction)
            {
                case ListDirection.FUTURE:
                    BillPays = await _context.BillPay
                        .Include(b => b.User)
                        .Include(b => b.Payments)
                            .ThenInclude(p => p.BillInstance)
                        .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                        .Where(b => b.BillPayDate >= DateTime.Today.AddDays(-1))
                        .OrderBy(b => b.BillPayDate)
                        .ToListAsync();
                    break;
                case ListDirection.RETRO:
                    BillPays = await _context.BillPay
                        .Include(b => b.User)
                        .Include(b => b.Payments)
                            .ThenInclude(p => p.BillInstance)
                        .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                        .Where(b => b.BillPayDate < DateTime.Today.AddDays(-1))
                        .OrderByDescending(b => b.BillPayDate)
                        .ToListAsync();
                    break;
            }

            
        }
    }
}
