namespace FinancesManagment.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinancesManagment.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "FinancesManagment.Models.ApplicationDbContext";
            AutomaticMigrationDataLossAllowed = true;
            
        }

        protected override void Seed(FinancesManagment.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.FinancialAccountRoles.AddOrUpdate(
                role => role.Id,
                new Models.FinancialAccountRole
                {
                    Id = 1,
                    Title = "Owner"
                },
                new Models.FinancialAccountRole
                {
                    Id = 2,
                    Title = "User"
                },
                new Models.FinancialAccountRole
                {
                    Id = 3,
                    Title = "Analyst"
                }
            );
        }
    }
}
