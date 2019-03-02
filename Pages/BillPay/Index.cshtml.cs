using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleBillPay;
using SimpleBillPay.Models;
using SimpleBillPay.Services;


namespace SimpleBillPay.Pages.BillPay
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly BillPayService _billPayService;
        public IndexModel(BillPayService billPayService)
        {
            _billPayService = billPayService;
        }

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
                    BillPays = 
                        await _billPayService.GetScheduledBillPaysAsync(((int)pageNumber - 1) * 10, 10);
                    TotalPages = 
                        (int)Math.Ceiling((double)await _billPayService.CountScheduledBillPaysAsync() / 10);
                    break;
                case ListDirection.RETRO:
                    BillPays = 
                        await _billPayService.GetHistoricalBillPaysAsync(((int)pageNumber - 1) * 10, 10);
                    TotalPages = 
                        (int)Math.Ceiling((double)await _billPayService.CountHistoricalBillPaysAsync() / 10);
                    break;
            }

            
        }
    }
}
