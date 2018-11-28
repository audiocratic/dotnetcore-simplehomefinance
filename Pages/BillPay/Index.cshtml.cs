using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using Microsoft.AspNetCore.Authorization;

namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class IndexModel : BillPayPageModel
    {
        public IndexModel(SimpleBillPay.BudgetContext context) : base(context) {}

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public ListDirection Direction { get; set; }

        public enum ListDirection
        {
            RETRO, FUTURE
        }

        public IList<SimpleBillPay.Models.BillPay> BillPays { get; set; }

        public async Task OnGetAsync(string direction, int? pageNumber)
        {
            pageNumber = pageNumber ?? 1;
            CurrentPage = (int)pageNumber;
            
            if(direction == null || direction.ToLower() == ListDirection.FUTURE.ToString().ToLower())
            {
                Direction = ListDirection.FUTURE;
            }
            else
            {
                Direction = ListDirection.RETRO;
            }

            switch(Direction)
            {
                case ListDirection.FUTURE:
                    BillPays = await GetScheduledBillPays(((int)pageNumber - 1) * 10, 10);
                    TotalPages = (int)Math.Ceiling((double)await CountScheduledBillPays() / 10);
                    break;
                case ListDirection.RETRO:
                    BillPays = await GetHistoricalBillPays(((int)pageNumber - 1) * 10, 10);
                    TotalPages = (int)Math.Ceiling((double)await CountHistoricalBillPays() / 10);
                    break;
            }

            
        }
    }
}
