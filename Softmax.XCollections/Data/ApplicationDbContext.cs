using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Softmax.XCollections.Models;
using Softmax.XCollections.Data.Entities;


namespace Softmax.XCollections.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<DepositConfirm> DepositConfirms { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<RefundConfirm> RefundConfirms { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanApproval> LoanApprovals { get; set; }
        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
