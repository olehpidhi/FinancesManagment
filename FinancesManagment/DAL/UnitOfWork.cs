using System;
using FinancesManagment.Models;

namespace FinancesManagment.DAL
{
    public class UnitOfWork : IDisposable
    {
        private ApplicationDbContext context = ApplicationDbContext.Create();
        private GenericRepository<FinancialAccount> financialAccountRepository;
        private GenericRepository<FinancialAccountMember> financialAccountMemberRepository;
        private GenericRepository<FinancialAccountRole> financialAccountRoleRepository;
        private GenericRepository<ApplicationUser> userRepository;
        private GenericRepository<Permission> permissionRepository;
        private GenericRepository<MemberPermission> memberPermissionRepository;
        private GenericRepository<Transaction> transactionRepository;
        private GenericRepository<Currency> currencyRepository;


        public GenericRepository<Currency> CurrencyRepository
        {
            get
            {

                if (this.currencyRepository == null)
                {
                    this.currencyRepository = new GenericRepository<Currency>(context);
                }
                return currencyRepository;
            }
        }

        public GenericRepository<ApplicationUser> UserRepository
        {
            get
            {

                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<ApplicationUser>(context);
                }
                return userRepository;
            }
        }

        public GenericRepository<Transaction> TransactionsRepository
        {
            get
            {

                if (this.transactionRepository == null)
                {
                    this.transactionRepository = new GenericRepository<Transaction>(context);
                }
                return transactionRepository;
            }
        }

        public GenericRepository<Permission> PermissionsRepository
        {
            get
            {

                if (this.permissionRepository == null)
                {
                    this.permissionRepository = new GenericRepository<Permission>(context);
                }
                return permissionRepository;
            }
        }

        public GenericRepository<MemberPermission> MemberPermissionsRepository
        {
            get
            {

                if (this.memberPermissionRepository == null)
                {
                    this.memberPermissionRepository = new GenericRepository<MemberPermission>(context);
                }
                return memberPermissionRepository;
            }
        }

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

        public GenericRepository<FinancialAccountMember> FinancialAccountMembersRepository
        {
            get
            {

                if (this.financialAccountMemberRepository == null)
                {
                    this.financialAccountMemberRepository = new GenericRepository<FinancialAccountMember>(context);
                }
                return financialAccountMemberRepository;
            }
        }

        public GenericRepository<FinancialAccountRole> FinancialAccountRolesRepository
        {
            get
            {

                if (this.financialAccountRoleRepository == null)
                {
                    this.financialAccountRoleRepository = new GenericRepository<FinancialAccountRole>(context);
                }
                return financialAccountRoleRepository;
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