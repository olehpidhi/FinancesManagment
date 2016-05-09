using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinancesManagment.Models;

namespace FinancesManagment.DAL
{
    public interface IFinancialAccountRepository: IDisposable
    {
        IEnumerable<FinancialAccount> GetFinancialAccounts();
        FinancialAccount GetFinancialAccountById(int id);
        void AddFinancialAccount(FinancialAccount account);
        void DeleteFinancialAccount(int id);
        void UpdateFinancialAccount(FinancialAccount account);
        void Save();
    }
}