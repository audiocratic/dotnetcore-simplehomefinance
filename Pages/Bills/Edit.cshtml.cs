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
using SimpleBillPay.Services;

namespace SimpleBillPay.Pages.Bills
{
    [Authorize]
    public class EditModel : PageModel
    {   
        private readonly BillService _billService;
        public EditModel(BillService billService)
        {
            _billService = billService;
        }

        [BindProperty]
        public BillInstance BillInstance { get; set; }

        [BindProperty]
        public bool ChangeEntireSeries { get; set; }

        public int? ReturnBillPayID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? returnBillPayID)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillInstance = await _billService.GetBillInstanceAsync((int)id);

            if (BillInstance == null)
            {
                return NotFound();
            }

            ReturnBillPayID = returnBillPayID ?? 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, int? returnBillPayID)
        {
            if (!ModelState.IsValid || id == null)
            {
                return Page();
            }

            BillInstance instance = await _billService.GetBillInstanceAsync((int)id);

            if(instance == null) return NotFound(); //Make sure this bill is in the DB and accessible to user

            //Overwrite properties from form
            instance.Amount = BillInstance.Amount;
            instance.DueDate = BillInstance.DueDate;
            instance.Name = BillInstance.BillTemplate.Name;

            bool isTemplated = 
                ((ChangeEntireSeries && BillInstance.BillTemplate.FrequencyInMonths > 0) 
                || (instance.BillTemplate.FrequencyInMonths == 0 
                    && BillInstance.BillTemplate.FrequencyInMonths > 0));

            if(isTemplated) 
                instance.BillTemplate.FrequencyInMonths = BillInstance.BillTemplate.FrequencyInMonths;
            
            instance.BillTemplate.Name = BillInstance.BillTemplate.Name;


            if(ChangeEntireSeries)
            {
                await _billService.RebuildSeriesAsync(instance);
                if(BillInstance.BillTemplate.FrequencyInMonths > 0) 
                    await _billService.CreateSeriesAsync(instance);
            }
            
            try
            {
                await _billService.UpdateAsync(instance);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _billService.BillInstanceExistsAsync(BillInstance.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if(returnBillPayID != null && returnBillPayID > 0)
            {
                return RedirectToPage("/BillPay/Edit", new {id = returnBillPayID} );
            }

            return RedirectToPage("./Index");            
        }

        
    }
}
