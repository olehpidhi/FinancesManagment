using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class FinancialAccountMember
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public FinancialAccount Account { get; set; }
        public FinancialAccountRole Role { get; set; }
    }
}