using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class EditModel : BillPageModel
    {   
        public EditModel(SimpleBillPay.BudgetContext context) : base(context)
        {
        }

        [BindProperty]
        public BillInstance BillInstance { get; set; }

        [BindProperty]
        public bool ChangeEntireSeries { get; set; }

        

        public async Task RebuildSeries(BillInstance instance)
        {
            List<BillInstance> instances = await FetchEditableBillInstancesAsync(instance);

            _context.RemoveRange(instances);

            if(BillInstance.BillTemplate.FrequencyInMonths > 0) CreateSeries(instance);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillInstance = await GetBillInstanceAsync((int)id);

            if (BillInstance == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid || id == null)
            {
                return Page();
            }

            BillInstance instance = await GetBillInstanceAsync((int)id);

            if(instance == null) return NotFound(); //Make sure this bill is in the DB and accessible to user
            
            _context.Attach(instance).State = EntityState.Modified;

            //Overwrite properties from form
            instance.Amount = BillInstance.Amount;
            instance.DueDate = BillInstance.DueDate;
            instance.Name = BillInstance.BillTemplate.Name;

            bool isTemplated = ((ChangeEntireSeries && BillInstance.BillTemplate.FrequencyInMonths > 0) ||
            (instance.BillTemplate.FrequencyInMonths == 0 && BillInstance.BillTemplate.FrequencyInMonths > 0));

            if(isTemplated) instance.BillTemplate.FrequencyInMonths = BillInstance.BillTemplate.FrequencyInMonths;
            
            instance.BillTemplate.Name = BillInstance.BillTemplate.Name;


            if(ChangeEntireSeries)
            {
                await RebuildSeries(instance);
            }
            


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillInstanceExists(BillInstance.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BillInstanceExists(int id)
        {
            return _context.BillInstance.Any(e => e.ID == id);
        }
    }
}
