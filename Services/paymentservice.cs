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
    public class PaymentService
    {
        private readonly BudgetContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public PaymentService(BudgetContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            PaymentQueryBuilder queryBuilder = new PaymentQueryBuilder(_context);
            
            return await queryBuilder
                .QueryPayments()
                .FilterByUser(_httpContextAccessor.HttpContext.User.Identity.Name)
                .Query
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<List<Payment>> GetPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.BillInstance)
                .Include(p => p.BillPay)
                .Include(p => p.BillInstance.BillTemplate)
                .Include(p => p.BillInstance.BillTemplate.User)
                .Where(p => p.BillInstance.BillTemplate.User.UserName == 
                    _httpContextAccessor.HttpContext.User.Identity.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Attach(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PaymentExistsAsync(int id)
        {
            return await _context.Payments.AnyAsync(e => e.ID == id);
        }

        public async Task ConfirmPaymentAsync(Payment payment)
        {
            //Remove payment
            _context.Attach(payment).State = EntityState.Modified;
            
            payment.DateConfirmed = 
                ((payment.DateConfirmed != null
                    && payment.DateConfirmed > DateTime.MinValue) ? (DateTime?)null : DateTime.Now);
            
            await _context.SaveChangesAsync();
        }

        public async Task<List<Payment>> GetPaymentsByBillPayAsync(
            int billPayID, 
            int start, 
            int offset)
        {
            PaymentQueryBuilder queryBuilder = new PaymentQueryBuilder(_context);
            
            return await queryBuilder
                .QueryPayments()
                .FilterByUser(_httpContextAccessor.HttpContext.User.Identity.Name)
                .Query
                .Where(p => p.BillPayID == billPayID)
                .OrderBy(p => p.BillInstance.DueDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountPaymentsByBillPayAsync(int billPayID)
        {
            PaymentQueryBuilder queryBuilder = new PaymentQueryBuilder(_context);
            
            return await queryBuilder
                .QueryPayments()
                .FilterByUser(_httpContextAccessor.HttpContext.User.Identity.Name)
                .Query
                .Where(p => p.BillPayID == billPayID)
                .CountAsync();
        }

        public async Task<decimal> GetSumOfPaymentsByBillPayAsync(int billPayID)
        {
            PaymentQueryBuilder queryBuilder = new PaymentQueryBuilder(_context);
            
            return await queryBuilder
                .QueryPayments()
                .FilterByUser(_httpContextAccessor.HttpContext.User.Identity.Name)
                .Query
                .Where(p => p.BillPayID == billPayID)
                .SumAsync(p => p.Amount);

        }

        public async Task ToggleConfirmationAsync(Payment payment)
        {
            _context.Attach(payment).State = EntityState.Modified;
            
            payment.DateConfirmed = 
                ((payment.DateConfirmed != null
                    && payment.DateConfirmed > DateTime.MinValue) ? (DateTime?)null : DateTime.Now);
            
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }

        private class PaymentQueryBuilder
        {
            public IQueryable<Payment> Query { get; set; }
            private readonly BudgetContext _context;

            public PaymentQueryBuilder(
                BudgetContext context)
            {
                _context = context;
            }

            public PaymentQueryBuilder QueryPayments()
            {
                Query = _context.Payments
                    .Include(p => p.BillInstance)
                        .ThenInclude(b => b.Payments)
                    .Include(p => p.BillInstance.BillTemplate);

                return this;
            }

            public PaymentQueryBuilder FilterByUser(string userName)
            {
                Query.Include(p => p.BillInstance.BillTemplate.User)
                    .Where(p => p.BillInstance.BillTemplate.User.UserName == userName);

                return this;
            }
        }
    }
}