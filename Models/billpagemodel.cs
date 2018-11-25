using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SimpleBillPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class BillPageModel : PageModel 
{
    protected readonly SimpleBillPay.BudgetContext _context;

    public BillPageModel(SimpleBillPay.BudgetContext context) : base()
    {
        _context = context;
    }

    public async Task<BillInstance> GetBillInstanceAsync(int id)
    {
        BillInstance instance = await _context.BillInstance
            .Include(b => b.BillTemplate)
            .Include(p => p.Payments)
            .Include(b => b.BillTemplate.User)
            .Where(b => b.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
            .FirstOrDefaultAsync(m => m.ID == id);

        return instance;
    }

    public async Task<List<BillInstance>> FetchEditableBillInstancesAsync(BillInstance instance)
    {
        List<BillInstance> instances = await _context.BillInstance
            .Include(b => b.BillTemplate)
            .Include(p => p.Payments)
            .Include(b => b.BillTemplate.User)
            .Where(b =>
                b.BillTemplate.User.UserName == HttpContext.User.Identity.Name
                && b.Payments.Count == 0
                && b.BillTemplateID == instance.BillTemplateID
                && b.DueDate > instance.DueDate)
            .ToListAsync();

            return instances;
    }

    public void CreateSeries(BillInstance instance)
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

            _context.BillInstance.Add(instanceToAdd);
        }
    }
}