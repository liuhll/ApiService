using System.Collections.Generic;
using Swashbuckle.Swagger;

namespace Jueci.ApiService.Web
{
    public class CachingSwaggerProvider : ISwaggerProvider
    {
        private readonly ISwaggerProvider _swaggerProvider;

        public CachingSwaggerProvider(ISwaggerProvider defaultProvider)
        {
            _swaggerProvider = defaultProvider;
        }

        public SwaggerDocument GetSwagger(string rootUrl, string apiVersion)
        {
            var sd = _swaggerProvider.GetSwagger(rootUrl, apiVersion);

            var rmPaths = new List<string>();

            foreach (var path in sd.paths)
            {
                if (path.Key.Contains("Abp") || path.Key.Contains("TypeScript"))
                {
                    rmPaths.Add(path.Key);
                }
            }
            foreach (var rmpath in rmPaths)
            {
                if (sd.paths.ContainsKey(rmpath))
                {
                    sd.paths.Remove(rmpath);
                }

            }

            return sd;
        }
    }
}