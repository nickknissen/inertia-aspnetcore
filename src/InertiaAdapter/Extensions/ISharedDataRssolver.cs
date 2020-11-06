using System.Collections.Generic;
using System.Threading.Tasks;

namespace InertiaAdapter.Extensions
{
    public interface ISharedDataResolver
    {
        public Task<Dictionary<string, object>> ResolveDataAsync(Microsoft.AspNetCore.Http.HttpContext httpContext);
    }
}