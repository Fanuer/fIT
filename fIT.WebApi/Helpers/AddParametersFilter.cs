using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using Swashbuckle.Swagger;

namespace fIT.WebApi.Helpers
{
    public class AddParametersFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {

            var hasAuthorizeAttr = apiDescription.ActionDescriptor.GetFilterPipeline()
                .Select(filterInfo => filterInfo.Instance)
                .Any(filter => filter is IAuthorizationFilter);

            var hasAllowAnonymous = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (hasAuthorizeAttr && !hasAllowAnonymous)
            {
                if (operation.parameters == null)
                {
                    operation.parameters = new List<Parameter>();
                }
                operation.parameters.Add(new Parameter()
                {
                    description = "Authorization token. Used for applying content access restrictions. Use one of the OAuth2 grants to auto-populate this value.",
                    @in = "header",
                    name = "Authorization",
                    required = true,
                    type = "string",
                    @default = "bearer "
                });
            }
        }
    }
}
