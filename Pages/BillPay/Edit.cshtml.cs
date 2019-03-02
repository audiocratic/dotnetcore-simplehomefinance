using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;
using SimpleBillPay.Services;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly BillPayService _billPayService;
        private readonly PaymentService _paymentService;
        private readonly ExpenseService _expenseService;
        private readonly BillService _billService;

        public EditModel(
            BillPayService billPayService, 
            PaymentService paymentService,
            ExpenseService expenseService,
            BillService billService)
        {
            _billPayService = billPayService;
            _paymentService = paymentService;
            _expenseService = expenseService;
            _billService = billService;
        }

        [BindProperty]
        public SimpleBillPay.Models.BillPay BillPay { get; set; }

        public int TotalBillPages { get; set; }
        public int CurrentBillPage { get; set; }
        public List<BillInstance> UpcomingBills { get; set; }


        public int TotalPaymentPages { get; set; }
        public int CurrentPaymentPage { get; set; }
        
        public int TotalExpensePages { get; set; }
        public int CurrentExpensePage { get; set; }

        public DateTime? DuplicateDateError { get; set; }
        


        [HttpPost]
        public IActionResult OnPostTest()
        {
            BillInstance instance = new BillInstance();

            instance.Name = HttpContext.User.Identity.Name;

            return new JsonResult(instance);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostConfirmPayment (int? paymentID, int? billPayID, [FromQuery]BillPayPaging paging)
        {

            if(billPayID == null || paymentID == null)
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            Payment payment = await _paymentService.GetPaymentByIdAsync((int)paymentID);

            if(payment == null) return NotFound();

            //Update payment confirmation status
            await _paymentService.ToggleConfirmationAsync(payment);

            //Call get method to rebuild page
           return RedirectToPage("Edit", new {
               ID = billPayID,
               PaymentPage = paging.PaymentPage,
               BillPage = paging.BillPage,
               ExpensePage = paging.ExpensePage
           });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostConfirmExpense (int? expenseID, int? billPayID, [FromQuery]BillPayPaging paging)
        {

            if(billPayID == null || expenseID == null)
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            Expense expense = await _expenseService.GetExpenseByIDAsync((int)expenseID);

            if(expense == null) return NotFound();

            //Remove expense
            await _expenseService.ToggleConfirmationAsync(expense);

            //Call get method to rebuild page
           return RedirectToPage("Edit", new {
               ID = billPayID,
               PaymentPage = paging.PaymentPage,
               BillPage = paging.BillPage,
               ExpensePage = paging.ExpensePage
           });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostRemovePayment (int? paymentID, int? billPayID)
        {
            if(billPayID == null || paymentID == null)
            {
                return NotFound();
            }

            //Ensure parameters belong to current user
            Payment payment = await _paymentService.GetPaymentByIdAsync((int)paymentID);

            if(payment == null) return NotFound();

            //Remove payment
            await _paymentService.RemoveAsync(payment);

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
            BillInstance instance = await _billService.GetBillInstanceAsync((int)billInstanceID);

            if(instance == null) return NotFound();

            SimpleBillPay.Models.BillPay billPay = await _billPayService.GetByIdAsync((int)billPayID);

            if(billPay == null) return NotFound();

            //Add new payment with this bill instance and attached to this bill pay
            Payment payment = new Payment();

            payment.BillInstance = instance;

            payment.Amount = (instance.Amount - instance.Payments.Sum(p => p.Amount));
            payment.PaymentDate = billPay.BillPayDate;
            payment.DateConfirmed = null;
            payment.BillPay = billPay;

            await _paymentService.AddAsync(payment);

            //Call get method to rebuild page
            return Redirect("./Edit?id=" + billPayID.ToString());
        }

        public async Task<IActionResult> OnGetAsync(int? id, DateTime? duplicateDateError, [FromQuery]BillPayPaging paging)
        {
            if (id == null)
            {
                return NotFound();
            }

            CurrentPaymentPage = paging.PaymentPage ?? 1;
            CurrentBillPage = paging.BillPage ?? 1;
            CurrentExpensePage = paging.ExpensePage ?? 1;

            if(duplicateDateError != null) DuplicateDateError = duplicateDateError;

            BillPay = await _billPayService.GetByIdAsync((int)id);

            if(BillPay == null) return NotFound();
            
            //Bills pagination
            int totalUpcomingBills = await _billService.CountUpcomingBillsAsync();
            TotalBillPages = (int)Math.Ceiling(((float)totalUpcomingBills / 20));
            UpcomingBills = await _billService.GetUpcomingBillsAsync((CurrentBillPage - 1) * 20, 20);
            
            //Payments pagination
            int totalPayments = await _paymentService.CountPaymentsByBillPayAsync(BillPay.ID);
            TotalPaymentPages = (int)Math.Ceiling(((float)totalPayments / 8));
            BillPay.Payments = 
                await _paymentService.GetPaymentsByBillPayAsync(BillPay.ID, (CurrentPaymentPage - 1) * 8, 8);

            //Expenses pagination
            int totalExpenses = await _expenseService.CountExpensesByBillPayAsync(BillPay.ID);
            TotalExpensePages = (int)Math.Ceiling(((float)totalExpenses / 8));
            BillPay.Expenses = await _expenseService.GetExpensesByBillPayAsync(BillPay.ID, (CurrentExpensePage - 1) * 8, 8);

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

            SimpleBillPay.Models.BillPay billPay = await _billPayService.GetByIdAsync(BillPay.ID);
            
            if(billPay == null) return NotFound();

            billPay.StartingAmount = BillPay.StartingAmount;
            billPay.BillPayDate = BillPay.BillPayDate;

            try
            {
                await _billPayService.UpdateBillPayAsync(billPay);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _billPayService.BillPayExistsAsync(BillPay.ID))
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
    }

    public class BillPayPaging
    {
        public int? PaymentPage { get; set; }
        public int? BillPage { get; set; }
        public int? ExpensePage { get; set; }
    }
}
