using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Abp.EntityFramework;
using Jueci.ApiService.ApiAuthorization.Entities;
using Jueci.ApiService.Mappings;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.EntityFramework
{
    public class ApiServiceDbContext : AbpDbContext
    {
        //TODO: Define an IDbSet for each Entity...

        //Example:
        //public virtual IDbSet<User> Users { get; set; }

        public virtual IDbSet<AppKey> AppKeys { get; set; }

        public virtual IDbSet<UserInfo> UserInfos { get; set; }

        public virtual IDbSet<UserServiceSubscriptionInfo> UserServiceSubscriptionInfos { get; set; }

        public virtual IDbSet<ServicePrice> ServicePrices { get; set; }

        public virtual IDbSet<AgentInfo> AgentInfos { get; set; }

        public virtual IDbSet<AgentRole> AgentRoles { get; set; }

        public virtual IDbSet<UserServiceAuthInfo> UserServiceAuthInfos { get; set; }

        public virtual IDbSet<UserRecharge> UserRecharges { get; set; }

        public virtual IDbSet<ServiceInfo> ServiceInfos { get; set; }

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
            modelBuilder.Configurations.Add(new UserServiceSubscriptionInfoMap());
            modelBuilder.Configurations.Add(new ServicePriceMap());
            modelBuilder.Configurations.Add(new AgentInfoMap());
            modelBuilder.Configurations.Add(new AgentRoleMap());
            modelBuilder.Configurations.Add(new UserServiceAuthInfoMap());
            modelBuilder.Configurations.Add(new UserRechargeMap());
            modelBuilder.Configurations.Add(new ServiceInfoMap());
        }
    }
}
