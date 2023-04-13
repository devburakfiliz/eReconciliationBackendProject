using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework.Context
{
    public class ContextDb:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=BURAK;Database=eReconciliationDb; Trusted_Connection=True");
        }
        public DbSet<AccountReconciliationsDetail> AccountReconciliationsDetails { get; set; }
        public DbSet<AccountReconciliation> AccountReconciliations { get; set; }
        public DbSet<BaBsReconciliationDetail> BaBsReconciliationDetails { get; set; }
        public DbSet<BaBsReconciliation> BaBsReconciliations { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyAccount> CurrencyAccounts { get; set; }
        public DbSet<MailParameter> MailParameters { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MailTemplate> MailTemplates { get; set; }

    }
}
