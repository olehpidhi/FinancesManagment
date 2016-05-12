using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class FinancialAccountMember
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int FinancialAccountId { get; set; }
        public FinancialAccount FinancialAccount { get; set; }
        public int FinancialAccountRoleId { get; set; }
        public FinancialAccountRole FinancialAccountRole { get; set; }

    }
}