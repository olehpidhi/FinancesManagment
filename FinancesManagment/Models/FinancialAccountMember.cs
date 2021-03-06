﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class FinancialAccountMember
    {
        public int Id { get; set; }
        public decimal Quote { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual FinancialAccount FinancialAccount { get; set; }
        public virtual FinancialAccountRole FinancialAccountRole { get; set; }
        public virtual List<MemberPermission> MemberPermissions { get; set; }
        public virtual List<Transaction> Transactions { get; set; }


        public static IEnumerable<FinancialAccountRole> Roles = new List<FinancialAccountRole>
        {
            new FinancialAccountRole
            {
                Id = 2,
                Title = "User"
            },
            new FinancialAccountRole
            {
                Id = 3,
                Title = "Analyst"
            }
        };

        public static IEnumerable<Permission> Permissions = new List<Permission>
        {
            new Permission { Id = 1, Title = "Make transaction" },
            new Permission { Id = 3, Title = "View statistic" }
        };
    }
}