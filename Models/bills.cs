using System;
using System.Collections;
using System.Collections.Generic;
using SimpleBillPay.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace SimpleBillPay.Models
{
    public class BillTemplate
    {
        public int ID { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }

        [Range(0,100)]
        public int FrequencyInMonths { get; set; }

    }

    public class BillInstance
    {
        public int ID { get; set; }
        public int BillTemplateID { get; set; }
        public virtual BillTemplate BillTemplate { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public IList<Payment> Payments { get; set; }

    }

    public class Payment
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public int BillInstanceID { get; set; }
        public virtual BillInstance BillInstance { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}

