using FinancesManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesManagment.DAL
{
    public class Queries
    {
        static private UnitOfWork unitOfWork = new UnitOfWork();
        public static IEnumerable<ApplicationUser> GetAccountMembers(FinancialAccount account)
        {
            return (from acc in unitOfWork.FinancialAccountMembersRepository.Get() where acc.FinancialAccountId == account.Id select acc.ApplicationUser);
        }
    }
}