using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Abp.EntityFramework;
using Jueci.ApiService.ApiAuthorization.Entities;
using Jueci.ApiService.Mappings;

namespace Jueci.ApiService.EntityFramework
{
    public class ApiServiceDbContext : AbpDbContext
    {
        //TODO: Define an IDbSet for each Entity...

        //Example:
        //public virtual IDbSet<User> Users { get; set; }

          public virtual IDbSet<AppKey> AppKeys { get; set; }

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public ApiServiceDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in ApiServiceDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of ApiServiceDbContext since ABP automatically handles it.
         */
        public ApiServiceDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AppkeyMap());
        }
    }
}
