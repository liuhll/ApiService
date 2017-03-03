using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Jueci.ApiService.EntityFramework.Repositories
{
    public abstract class ApiServiceRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<ApiServiceDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ApiServiceRepositoryBase(IDbContextProvider<ApiServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class ApiServiceRepositoryBase<TEntity> : ApiServiceRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ApiServiceRepositoryBase(IDbContextProvider<ApiServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
