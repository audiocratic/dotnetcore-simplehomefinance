using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SimpleBillPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace SimpleBillPay.Services
{
    public class BillPayService
    {
        private readonly BudgetContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillPayService(BudgetContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddAsync(BillPay billPay)
        {
            _context.BillPay.Add(billPay);
            await _context.SaveChangesAsync();
        }

        public async Task<BillPay> GetByIdAsync(int id)
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Where(b => b.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task<List<SimpleBillPay.Models.BillPay>> GetScheduledBillPaysAsync(
            int start, int offset)
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                    .ThenInclude(p => p.BillInstance)
                .Where(b => b.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate >= DateTime.Today.AddDays(-1))
                .OrderBy(b => b.BillPayDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountScheduledBillPaysAsync()
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Where(b => b.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate >= DateTime.Today.AddDays(-1))
                .CountAsync();
        }

        public async Task<List<SimpleBillPay.Models.BillPay>> GetHistoricalBillPaysAsync(
            int start, int offset)
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                    .ThenInclude(p => p.BillInstance)
                .Where(b => b.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate < DateTime.Today.AddDays(-1))
                .OrderByDescending(b => b.BillPayDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountHistoricalBillPaysAsync()
        {
            return await _context.BillPay
                .Include(b => b.User)
                .Where(b => b.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(b => b.BillPayDate < DateTime.Today.AddDays(-1))
                .CountAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            BillPay billPay = await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Where(b => b.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(b => b.ID == (int)id);

            if (billPay != null)
            {
                billPay.Payments.ForEach(p => _context.Payments.Remove(p));
                billPay.Expenses.ForEach(e => _context.Expenses.Remove(e));
                
                _context.BillPay.Remove(billPay);

                await _context.SaveChangesAsync();
            }
        }
        
        public async Task UpdateBillPayAsync(BillPay billPay)
        {
            _context.Attach(billPay).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> BillPayExistsAsync(int id)
        {
            return await _context.BillPay.AnyAsync(e => e.ID == id);
        }

        
    }
}