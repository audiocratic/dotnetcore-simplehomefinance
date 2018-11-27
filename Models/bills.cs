using System;
using System.Collections;
using System.Collections.Generic;
using SimpleBillPay.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public DateTime? DateConfirmed { get; set; }
        public int? BillPayID { get; set; }
        public BillPay BillPay { get; set; }

        [NotMapped]
        public bool IsConfirmed
        {
            get
            {
                bool confirmed = false;

                if(DateConfirmed != null)
                {
                    confirmed = (DateConfirmed > DateTime.MinValue);
                }

                return confirmed;
            }
        }
        
        [NotMapped]
        public decimal PriorBalance
        {
            get
            {
                decimal balance = 0;

                if(BillInstance != null)
                {
                    balance = BillInstance.Amount;

                    if(BillInstance.Payments != null && BillInstance.Payments.Count > 0)
                    {
                        balance -= BillInstance.Payments
                            .Where(p => p.PaymentDate < PaymentDate
                                || (p.PaymentDate == PaymentDate && p.ID < ID))
                            .Sum(p => p.Amount);
                    }
                }

                return balance;
            }
        }
    }

    public class BillPay 
    {
        public int ID { get; set; }
        public DateTime BillPayDate { get; set; }
        public decimal StartingAmount { get; set; }
        public List<Payment> Payments { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public decimal EndingAmount
        {
            get
            {
                decimal amount = 0;

                if(Payments != null)
                {
                    amount = StartingAmount - Payments.Sum(p => p.Amount);
                }

                return amount;
            }
        }

    }
}

