using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class FinancialAccount
    {
        public FinancialAccount()
        {
            FinancialAccountMembers = new List<FinancialAccountMember>();
        }
        public virtual List<FinancialAccountMember> FinancialAccountMembers { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Summary { get; set; }
    }
}

