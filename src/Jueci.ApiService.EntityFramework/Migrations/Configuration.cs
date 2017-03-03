using System.Data.Entity.Migrations;

namespace Jueci.ApiService.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApiService.EntityFramework.ApiServiceDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ApiService";
        }

        protected override void Seed(ApiService.EntityFramework.ApiServiceDbContext context)
        {
            // This method will be called every time after migrating to the latest version.
            // You can add any seed data here...
        }
    }
}
