using BrazilianHedgeFunds.ETL.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace BrazilianHedgeFunds.ETL.Infrastructure.Repository
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) { }

        public DbSet<HedgeFundRecord> HedgeFundRecord { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HedgeFundRecord>().HasKey(fund => fund.Id);
            modelBuilder.Entity<HedgeFundRecord>().HasIndex(fund => fund.CNPJ);
            modelBuilder.Entity<HedgeFundRecord>().HasIndex(fund => fund.RecordDate);
        }
    }
}
