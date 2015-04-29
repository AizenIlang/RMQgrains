using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using RMQGrainsBeta.Migrations;

namespace RMQGrainsBeta.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        //Eto un pang AppHarbor

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new
                MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            base.OnModelCreating(modelBuilder);
        }

        public System.Data.Entity.DbSet<RMQGrainsBeta.Models.Cement> Cements { get; set; }

        public System.Data.Entity.DbSet<RMQGrainsBeta.Models.Delivery> Deliveries { get; set; }

        public System.Data.Entity.DbSet<RMQGrainsBeta.Models.Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<RMQGrainsBeta.Models.Payment> Payments { get; set; }

        public System.Data.Entity.DbSet<RMQGrainsBeta.Models.Expense> Expenses { get; set; }
    }
}