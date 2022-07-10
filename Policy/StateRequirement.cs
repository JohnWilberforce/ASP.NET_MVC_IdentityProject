using Microsoft.AspNetCore.Authorization;

namespace IdentityFromScratch.Policy
{
    public class SpecificStateRequirement : AuthorizationHandler<SpecificStateRequirement>, IAuthorizationRequirement
    {

        public string StateName { get; set; }

        public SpecificStateRequirement(string statename)
        {
            StateName = statename.ToLower();
            Console.WriteLine($"State Requirement for {StateName}");
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecificStateRequirement requirement)
        {
            
            if (context.User?.HasClaim(c => c.Type == "State" && (StateName == c.Value.ToLower())) ?? false)
            {
                context.Succeed(requirement);

            }
            else
            {
                HttpContext? _CTX = context.Resource as HttpContext;

                if (_CTX != null)
                {
                    _CTX.Response.Cookies.Append("AuthenticationFailureReason",
                    $"You are Required to be in the state '{StateName}' but you have not presented any claims proving that you are",
                    new CookieOptions() { MaxAge = TimeSpan.FromSeconds(10) });
                }
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
