using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class MemberPermission
    {
        public int Id { get; set; }
        public virtual Permission Permission { get; set; }
        public virtual FinancialAccountMember FinancialAccountMember { get; set; }
    }
}