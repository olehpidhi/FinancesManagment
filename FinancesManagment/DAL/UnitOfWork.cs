using System;
using FinancesManagment.Models;

namespace FinancesManagment.DAL
{
    public class UnitOfWork : IDisposable
    {
        private ApplicationDbContext context = ApplicationDbContext.Create();
        private GenericRepository<FinancialAccount> financialAccountRepository;

        public GenericRepository<FinancialAccount> FinancialAccountsRepository
        {
            get
            {

                if (this.financialAccountRepository == null)
                {
                    this.financialAccountRepository = new GenericRepository<FinancialAccount>(context);
                }
                return financialAccountRepository;
            }
        }

        public int Save()
        {
            return context.SaveChanges();
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