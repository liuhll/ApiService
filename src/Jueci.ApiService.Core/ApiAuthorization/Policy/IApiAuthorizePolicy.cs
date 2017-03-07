using Abp.Dependency;

namespace Jueci.ApiService.ApiAuthorization.Policy
{
    public interface IApiAuthorizePolicy : ITransientDependency
    {
        bool IsValidTime();

        bool IsLegalSign();
    }
}