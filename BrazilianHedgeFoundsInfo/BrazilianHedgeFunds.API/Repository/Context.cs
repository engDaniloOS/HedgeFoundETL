using BrazilianHedgeFunds.API.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace BrazilianHedgeFunds.API.Repository
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) { }

        public DbSet<HedgeFundRecord> HedgeFundRecord { get; set; }
    }
}
