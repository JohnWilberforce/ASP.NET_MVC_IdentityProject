using Microsoft.AspNetCore.Authorization;

namespace IdentityFromScratch.Policy
{
    public class SpecificUserRequirement : AuthorizationHandler<SpecificUserRequirement>, IAuthorizationRequirement
    {

        public string UserName { get; set; }
        public string Email { get; set; }
     

        public SpecificUserRequirement(string username)
        {
            UserName = username.ToLower();
           
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecificUserRequirement requirement)
        {
            
            if (context.User?.Identity?.Name?.ToLower()?.Equals(UserName) ?? false)
            {
                context.Succeed(requirement);

            }
            else
            {
                
                HttpContext? _CTX = context.Resource as HttpContext;
                
                if (_CTX != null)
                {
                    _CTX.Response.Cookies.Append("AuthenticationFailureReason",
                        $"You are Required to be the user '{UserName}' but it appears you are actually the user '{context.User?.Identity?.Name?.ToLower()}'",
                        new CookieOptions() { MaxAge = TimeSpan.FromSeconds(10) });
                }
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
