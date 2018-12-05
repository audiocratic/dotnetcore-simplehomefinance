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
using MySql.Data.MySqlClient;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class EditModel : BillPayPageModel
    {
        public EditModel(SimpleBillPay.BudgetContext context) : base(context) {}

        [BindProperty]
        public SimpleBillPay.Models.BillPay BillPay { get; set; }

        public int TotalBillPages { get; set; }
        public int CurrentBillPage { get; set; }
        public List<BillInstance> UpcomingBills { get; set; }


        public int TotalPaymentPages { get; set; }
        public int CurrentPaymentPage { get; set; }

        public DateTime? DuplicateDateError { get; set; }
        


        [HttpPost]
        public IActionResult OnPostTest()
        {
            BillInstance instance = new BillInstance();

            instance.Name = HttpContext.User.Identity.Name;

            return new JsonResult(instance);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostConfirmPayment (int? paymentID, int? billPayID)
        {

            if(billPayID == null || paymentID == null)
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            Payment payment = await GetPaymentById((int)paymentID);

            if(payment == null) return NotFound();

            //Remove payment
            _context.Attach(payment).State = EntityState.Modified;
            
            payment.DateConfirmed = 
                ((payment.DateConfirmed != null
                    && payment.DateConfirmed > DateTime.MinValue) ? (DateTime?)null : DateTime.Now);
            
            await _context.SaveChangesAsync();

            //Call get method to rebuild page
           return Redirect("./Edit?id=" + billPayID.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> OnPostRemovePayment (int? paymentID, int? billPayID)
        {
            if(billPayID == null || paymentID == null)
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            Payment payment = await GetPaymentById((int)paymentID);

            if(payment == null) return NotFound();

            //Remove payment
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            //Call get method to rebuild page
           return Redirect("./Edit?id=" + billPayID.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAddPayment(int? billInstanceID, int? billPayID)
        {            

            if(billPayID == null || billInstanceID == null)
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            BillInstance instance = await _context.BillInstance
                .Include(b => b.BillTemplate)
                .Include(b => b.BillTemplate.User)
                .Include(b => b.Payments)
                .Where(b => b.BillTemplate.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(b => b.ID == billInstanceID);

            if(instance == null) return NotFound();

            SimpleBillPay.Models.BillPay billPay = await _context.BillPay
                .Include(b => b.User)
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(b => b.ID == billPayID);

            if(billPay == null) return NotFound();

            //Add new payment with this bill instance and attached to this bill pay
            Payment payment = new Payment();

            payment.BillInstance = instance;

            payment.Amount = (instance.Amount - instance.Payments.Sum(p => p.Amount));
            payment.PaymentDate = billPay.BillPayDate;
            payment.DateConfirmed = null;
            payment.BillPay = billPay;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            

            //Call get method to rebuild page
            return Redirect("./Edit?id=" + billPayID.ToString());
        }

        public async Task<IActionResult> OnGetAsync(
            int? id, 
            DateTime? duplicateDateError,
            int? paymentPage,
            int? billPage)
        {
            if (id == null)
            {
                return NotFound();
            }

            CurrentPaymentPage = paymentPage ?? 1;
            CurrentBillPage = billPage ?? 1;

            if(duplicateDateError != null) DuplicateDateError = duplicateDateError;

            BillPay = await _context.BillPay
                .Where(b => b.User.UserName == HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync(m => m.ID == id);

            if(BillPay == null) return NotFound();
            
            //Bills pagination
            int totalUpcomingBills = await CountUpcomingBills();

            TotalBillPages = (int)Math.Ceiling(((float)totalUpcomingBills / 20));
            
            UpcomingBills = await GetUpcomingBills((CurrentBillPage - 1) * 20, 20);
            
            //Payments pagination
            int totalPayments = await CountPaymentsByBillPay(BillPay.ID);

            TotalPaymentPages = (int)Math.Ceiling(((float)totalPayments / 15));

            BillPay.Payments = await GetPaymentsByBillPay(BillPay.ID, (CurrentPaymentPage - 1) * 15, 15);

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
            catch(DbUpdateException ex)
            {
                if(ex.InnerException is MySqlException)
                {
                    if(((MySqlException)ex.InnerException).Number == 1062)
                    {
                        return RedirectToPage("./Edit", 
                            new 
                            {
                                id = BillPay.ID, 
                                duplicateDateError = BillPay.BillPayDate.ToString("yyyy-MM-dd")
                            });
                    }
                }
            }

            return RedirectToPage("./Edit", new {id = BillPay.ID});
        }

        private bool BillPayExists(int id)
        {
            return _context.BillPay.Any(e => e.ID == id);
        }
    }
}
