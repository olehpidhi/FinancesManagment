using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class Transaction
    {
        public Transaction(decimal amount, string category, FinancialAccountMember member)
        {
            Amount = amount;
            Category = category;
            FinancialAccountMember = member;
        }
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public virtual FinancialAccountMember FinancialAccountMember { get; set; }
    }
}