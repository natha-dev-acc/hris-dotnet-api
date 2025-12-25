using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace HRIS_API.Swagger
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // cek apakah endpoint pakai [Authorize]
            var hasAuthorize =
                context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>().Any()
                || context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>().Any();

            if (!hasAuthorize)
                return;

            operation.Security ??= new System.Collections.Generic.List<OpenApiSecurityRequirement>();

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [scheme] = new string[] { }
            });
        }
    }
}
