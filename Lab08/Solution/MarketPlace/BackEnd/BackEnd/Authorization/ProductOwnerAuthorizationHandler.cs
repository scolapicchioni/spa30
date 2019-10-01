using BackEnd.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BackEnd.Authorization {
    public class ProductOwnerAuthorizationHandler : AuthorizationHandler<ProductOwnerAuthorizationRequirement, Product> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProductOwnerAuthorizationRequirement requirement, Product resource) {
            if (!context.User.HasClaim(c => c.Type == JwtClaimTypes.Name && c.Issuer == "http://localhost:5000"))
                return Task.CompletedTask;

            var userName = context.User.FindFirst(c => c.Type == JwtClaimTypes.Name && c.Issuer == "http://localhost:5000").Value;

            if (userName == resource.UserName)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
