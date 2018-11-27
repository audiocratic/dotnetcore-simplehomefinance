using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;

namespace SimpleBillPay.Pages.Payments
{
    public class IndexModel : PageModel
    {
        private readonly SimpleBillPay.BudgetContext _context;

        public IndexModel(SimpleBillPay.BudgetContext context)
        {
            _context = context;
        }

        public IList<Payment> Payment { get;set; }

        public async Task OnGetAsync()
        {
            Payment = await _context.Payments
                .Include(p => p.BillInstance)
                .Include(p => p.BillPay).ToListAsync();
        }
    }
}
