using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;

namespace FinancesManagment.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public ApplicationUser()
        {
            FinancialAccountMembers = new List<FinancialAccountMember>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public string Name { get; set; }
        public virtual List<FinancialAccountMember> FinancialAccountMembers { get; set; }
        public string Surname { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            // Database.SetInitializer(new DropCreateDatabaseAlways<ApplicationDbContext>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<FinancialAccount> FinancialAccounts { get; set; }
        public DbSet<FinancialAccountRole> FinancialAccountRoles { get; set; }
        public DbSet<FinancialAccountMember> FinancialAccountMembers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<MemberPermission> MemberPermissions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
    }
}