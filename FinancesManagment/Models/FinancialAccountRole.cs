using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class FinancialAccountRole
    {
        public FinancialAccountRole()
        {
            FinancialAccountMembers = new List<FinancialAccountMember>();
        }
        public virtual List<FinancialAccountMember> FinancialAccountMembers { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
    }
}