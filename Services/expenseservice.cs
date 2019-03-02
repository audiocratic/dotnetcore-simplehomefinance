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
    public class ExpenseService
    {
        private readonly BudgetContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public ExpenseService(BudgetContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Expense> GetExpenseByIDAsync(int id)
        {
            Expense expense = await _context.Expenses
                .Include(e => e.BillPay)
                .Where(e => e.BillPay.User.UserName == 
                    _httpContextAccessor.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(e => e.ID == id);

            return expense;
        }

        public async Task<List<Expense>> GetExpensesByBillPayAsync(
            int billPayID, 
            int start, 
            int offset)
        {
            return await _context.Expenses
                .Include(e => e.BillPay.User)
                .Where(e => e.BillPay.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(e => e.BillPay.ID == billPayID)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountExpensesByBillPayAsync(int billPayID)
        {
            return await _context.Expenses
                .Include(e => e.BillPay.User)
                .Where(e => e.BillPay.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(e => e.BillPay.ID == billPayID)
                .CountAsync();
        }

        public async Task<Decimal> GetSumOfExpensesByBillPayAsync(int billPayID)
        {
            return await _context.Expenses
                .Include(e => e.BillPay.User)
                .Where(e => e.BillPay.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(e => e.BillPay.ID == billPayID)
                .SumAsync(e => e.Amount);
        }

        public async Task ToggleConfirmationAsync(Expense expense)
        {
            //Remove payment
            _context.Attach(expense).State = EntityState.Modified;
            
            expense.DateConfirmed = 
                ((expense.DateConfirmed != null) ? (DateTime?)null : DateTime.Now);
            
            await _context.SaveChangesAsync();
        }
    }
}