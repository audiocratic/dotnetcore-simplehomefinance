using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SimpleBillPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class BillPayPageModel : PageModel
    {
        protected readonly SimpleBillPay.BudgetContext _context;

        public BillPayPageModel(SimpleBillPay.BudgetContext context) : base()
        {
            _context = context;
        }

        public async Task<List<BillInstance>> GetUpcomingBills(int start, int offset)
        {
            return await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(b => b.BillTemplate.User)
                .Include(b => b.Payments)
                .Where(b => b.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .Where(b => b.DueDate <= DateTime.Today.AddYears(2))
                .Where(b => b.Payments.Count == 0 || b.Payments.Sum(p => p.Amount) < b.Amount)
                .OrderBy(b => b.DueDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountUpcomingBills()
        {
            return await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(b => b.BillTemplate.User)
                .Include(b => b.Payments)
                .Where(b => b.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .Where(b => b.DueDate <= DateTime.Today.AddYears(2))
                .Where(b => b.Payments.Count == 0 || b.Payments.Sum(p => p.Amount) < b.Amount)
                .CountAsync();
        }

        public async Task<Payment> GetPaymentById(int id)
        {
            Payment payment = await _context.Payments
                .Include(p => p.BillInstance)
                .Include(p => p.BillInstance.BillTemplate)
                .Include(p => p.BillInstance.BillTemplate.User)
                .Where(b => b.BillInstance.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(b => b.ID == id);

            return payment;
        }

        public async Task<List<Payment>> GetPaymentsByBillPay(int billPayID, int start, int offset)
        {
            return await _context.Payments
                .Include(p => p.BillInstance)
                .Include(p => p.BillInstance.BillTemplate)
                .Include(p => p.BillInstance.BillTemplate.User)
                .Where(p => p.BillInstance.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .Where(p => p.BillPayID == billPayID)
                .OrderBy(p => p.BillInstance.DueDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountPaymentsByBillPay(int billPayID)
        {
            return await _context.Payments
                .Include(p => p.BillInstance)
                .Include(p => p.BillInstance.BillTemplate)
                .Include(p => p.BillInstance.BillTemplate.User)
                .Where(p => p.BillInstance.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .Where(p => p.BillPayID == billPayID)
                .CountAsync();
        }

        public async Task<decimal> GetSumOfPaymentsByBillPay(int billPayID)
        {
            return await _context.Payments
                .Include(p => p.BillPay)
                .Include(p => p.BillInstance.BillTemplate.User)
                .Where(p => p.BillInstance.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .Where(p => p.BillPayID == billPayID)
                .SumAsync(p => p.Amount);

        }

        public async Task<List<SimpleBillPay.Models.BillPay>> GetScheduledBillPays(int start, int offset)
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                    .ThenInclude(p => p.BillInstance)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate >= DateTime.Today.AddDays(-1))
                .OrderBy(b => b.BillPayDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountScheduledBillPays()
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate >= DateTime.Today.AddDays(-1))
                .CountAsync();
        }

        public async Task<List<SimpleBillPay.Models.BillPay>> GetHistoricalBillPays(int start, int offset)
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                    .ThenInclude(p => p.BillInstance)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate < DateTime.Today.AddDays(-1))
                .OrderByDescending(b => b.BillPayDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountHistoricalBillPays()
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate < DateTime.Today.AddDays(-1))
                .CountAsync();
        }
    }
}

