using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesManagment.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Title { get;set; }
        public virtual List<MemberPermission> MemberPermissions { get; set; }
    }
}