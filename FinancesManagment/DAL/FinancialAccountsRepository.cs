using FinancesManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesManagment.DAL
{
    public class FinancialAccountsRepository: IFinancialAccountRepository, IDisposable
    {
        private ApplicationDbContext context;

        public FinancialAccountsRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<FinancialAccount> GetFinancialAccounts()
        {
            return context.FinancialAccounts.ToList();
        }

        public FinancialAccount GetFinancialAccountById(int id)
        {
            return context.FinancialAccounts.Where(f => f.Id == id).First();
        }

        public void AddFinancialAccount(FinancialAccount account)
        {
            context.FinancialAccounts.Add(account);
        }

        public void DeleteFinancialAccount(int id)
        {
            FinancialAccount account = GetFinancialAccountById(id);
            context.FinancialAccounts.Remove(account);
        }

        public void UpdateFinancialAccount(FinancialAccount acconut)
        {
            context.Entry(acconut).State = System.Data.Entity.EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}