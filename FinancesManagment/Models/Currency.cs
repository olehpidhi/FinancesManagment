using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public virtual List<FinancialAccount> FinancialAccounts { get; set; }
    }
}