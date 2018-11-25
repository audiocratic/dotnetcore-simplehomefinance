using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using Microsoft.AspNetCore.Authorization;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class EditModel : BillPayPageModel
    {
        public EditModel(SimpleBillPay.BudgetContext context) : base(context) {}

        [BindProperty]
        public SimpleBillPay.Models.BillPay BillPay { get; set; }
        
        public List<BillInstance> UpcomingBills { get; set; }

        [HttpPost]
        public async Task<IActionResult> OnPostConfirmPayment (string paymentID, string billPayID)
        {
            //Ensure parameters have been supplied            
            int parsedPaymentID;
            int parsedBillPayID;

            if(billPayID == null 
                || paymentID == null
                || !int.TryParse(paymentID, out parsedPaymentID)
                || !int.TryParse(billPayID, out parsedBillPayID))
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            Payment payment = await GetPaymentById(parsedPaymentID);

            if(payment == null) return NotFound();

            //Remove payment
            _context.Attach(payment).State = EntityState.Modified;
            
            payment.DateConfirmed = (payment.DateConfirmed > DateTime.MinValue ? DateTime.MinValue : DateTime.Now);
            
            await _context.SaveChangesAsync();

            //Call get method to rebuild page
           return Redirect("./Edit?id=" + parsedBillPayID.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> OnPostRemovePayment (string paymentID, string billPayID)
        {
            //Ensure parameters have been supplied            
            int parsedPaymentID;
            int parsedBillPayID;

            if(billPayID == null 
                || paymentID == null
                || !int.TryParse(paymentID, out parsedPaymentID)
                || !int.TryParse(billPayID, out parsedBillPayID))
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            Payment payment = await GetPaymentById(parsedPaymentID);

            if(payment == null) return NotFound();

            //Remove payment
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            //Call get method to rebuild page
           return Redirect("./Edit?id=" + parsedBillPayID.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAddPayment(string billInstanceID, string billPayID)
        {            
            //Ensure parameters have been supplied            
            int parsedInstanceID;
            int parsedBillPayID;

            if(billPayID == null 
                || billInstanceID == null
                || !int.TryParse(billInstanceID, out parsedInstanceID)
                || !int.TryParse(billPayID, out parsedBillPayID))
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            BillInstance instance = await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(b => b.BillTemplate.User)
                .Include(b => b.Payments)
                .Where(b => b.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(b => b.ID == parsedInstanceID);

            if(instance == null) return NotFound();

            SimpleBillPay.Models.BillPay billPay = await _context.BillPay
                .Include(b => b.User)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(b => b.ID == parsedBillPayID);

            if(billPay == null) return NotFound();

            //Add new payment with this bill instance and attached to this bill pay
            Payment payment = new Payment();

            payment.BillInstance = instance;

            payment.Amount = (instance.Amount - instance.Payments.Sum(p => p.Amount));
            payment.PaymentDate = billPay.BillPayDate;
            payment.BillPay = billPay;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            

            //Call get method to rebuild page
            return Redirect("./Edit?id=" + parsedBillPayID.ToString());
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BillPay = await _context.BillPay
                .Include(b => b.Payments)
                    .ThenInclude(p => p.BillInstance)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == id);

            if(BillPay == null) return NotFound();

            UpcomingBills = await GetUpcomingBills();

            if (BillPay == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SimpleBillPay.Models.BillPay billPay = await _context.BillPay
                .Include(b => b.User)
                .Include(b => b.Payments)
                    .ThenInclude(p => p.BillInstance)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == BillPay.ID);
            
            if(billPay == null) return NotFound();

            _context.Attach(billPay).State = EntityState.Modified;

            billPay.StartingAmount = BillPay.StartingAmount;
            billPay.BillPayDate = BillPay.BillPayDate;

            foreach(Payment payment in billPay.Payments)
            {
                Payment formPayment = BillPay.Payments.Where(p => p.ID == payment.ID).FirstOrDefault();

                if(formPayment != null)
                {
                    payment.Amount = formPayment.Amount;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillPayExists(BillPay.ID))
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

        private bool BillPayExists(int id)
        {
            return _context.BillPay.Any(e => e.ID == id);
        }
    }
}
