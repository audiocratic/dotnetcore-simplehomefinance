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

    public class BillService
    {
        private readonly BudgetContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillService(BudgetContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddAsync(BillInstance billInstance)
        {
            _context.BillInstance.Add(billInstance);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(BillInstance billInstance)
        {
            _context.BillInstance.Remove(billInstance);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BillInstance>> GetUpcomingBillsAsync(int start, int offset)
        {
            return await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(b => b.BillTemplate.User)
                .Include(b => b.Payments)
                .Where(b => b.BillTemplate.User.UserName == 
                    _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(b => b.DueDate <= DateTime.Today.AddYears(2))
                .Where(b => b.Payments.Count == 0 || b.Payments.Sum(p => p.Amount) < b.Amount)
                .OrderBy(b => b.DueDate)
                .Skip(start).Take(offset)
                .ToListAsync();
        }

        public async Task<int> CountUpcomingBillsAsync()
        {
            return await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(b => b.BillTemplate.User)
                .Include(b => b.Payments)
                .Where(b => b.BillTemplate.User.UserName == 
                   _httpContextAccessor.HttpContext.User.Identity.Name)
                .Where(b => b.DueDate <= DateTime.Today.AddYears(2))
                .Where(b => b.Payments.Count == 0 || b.Payments.Sum(p => p.Amount) < b.Amount)
                .CountAsync();
        }

        public async Task<BillInstance> GetBillInstanceAsync(int id)
        {
            BillInstance instance = await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(p => p.Payments)
                .Include(b => b.BillTemplate.User)
                .Where(b => b.BillTemplate.User.UserName == 
                    _httpContextAccessor.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == id);

            return instance;
        }

        public async Task<int> CountBillsOnOrBeforeDueDateAsync(DateTime dueDate)
        {
            return await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Where(b => 
                        b.BillTemplate.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name
                    &&  b.DueDate <= dueDate
                )
                .CountAsync();
        }

        public async Task<List<BillInstance>> GetBillsOnOrBeforeDueDateAsync(
            DateTime dueDate,
            int skip,
            int take)
        {
            return await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Where(b => 
                        b.BillTemplate.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name
                    &&  b.DueDate <= dueDate)
                .OrderBy(b => b.DueDate)
                .Skip(skip).Take(take)
                .ToListAsync();
        }

        public async Task<List<BillInstance>> FetchEditableBillInstancesAsync(BillInstance instance)
        {
            List<BillInstance> instances = await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(p => p.Payments)
                .Include(b => b.BillTemplate.User)
                .Where(b =>
                    b.BillTemplate.User.UserName == _httpContextAccessor.HttpContext.User.Identity.Name
                    && b.Payments.Count == 0
                    && b.BillTemplateID == instance.BillTemplateID
                    && b.DueDate > instance.DueDate)
                .ToListAsync();

                return instances;
        }

        public async Task RebuildSeriesAsync(BillInstance instance)
        {
            List<BillInstance> instances = 
                await FetchEditableBillInstancesAsync(instance);

            _context.RemoveRange(instances);
        }

        public async Task<bool> BillInstanceExistsAsync(int id)
        {
            return await _context.BillInstance.AnyAsync(e => e.ID == id);
        }

        public async Task UpdateAsync(BillInstance billInstance)
        {
            _context.Attach(billInstance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task CreateSeriesAsync(BillInstance instance)
        {
            //Make sure there are enough bills to last from now until
            //thirty years from now. If that ends up not being enough,
            //we can argue about it thirty years from now.

            DateTime thirtyYearsFromNow = DateTime.Today.AddYears(30);

            int instancesToAdd = Convert.ToInt16(
                (
                    (thirtyYearsFromNow - instance.DueDate).TotalDays / 365.25 * 12
                ) / instance.BillTemplate.FrequencyInMonths);

            for(int i = 1; i <= instancesToAdd; i++)
            {
                DateTime dueDate = instance.DueDate.AddMonths(i * instance.BillTemplate.FrequencyInMonths);

                BillInstance instanceToAdd = new BillInstance();

                instanceToAdd.BillTemplate = instance.BillTemplate;
                instanceToAdd.DueDate = dueDate;
                instanceToAdd.Amount = instance.Amount;
                instanceToAdd.Name = instance.Name;

                await _context.BillInstance.AddAsync(instanceToAdd);
            }
    }
    }
}