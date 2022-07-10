using Microsoft.AspNetCore.Authorization;

namespace IdentityFromScratch.Policy
{
    public class MinimumAgeRequirement : AuthorizationHandler<MinimumAgeRequirement>, IAuthorizationRequirement
    {

        public int MinimumAge { get; set; }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            try
            {
                if (context.User?.HasClaim(c => c.Type == "Age" && (int.Parse(c.Value) >= MinimumAge)) ?? false)
                {
                    context.Succeed(requirement);

                }
                else
                {
                    HttpContext? _CTX = context.Resource as HttpContext;

                    if (_CTX != null)
                    {
                        _CTX.Response.Cookies.Append("AuthenticationFailureReason",
                        $"You are Required to be at least '{MinimumAge}' but you have not presented any claims proving that you are",
                        new CookieOptions() { MaxAge = TimeSpan.FromSeconds(10) });
                    }
                    context.Fail();
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                HttpContext? _CTX = context.Resource as HttpContext;

                if (_CTX != null)
                {
                    _CTX.Response.Cookies.Append("AuthenticationFailureReason",
                    $"You are Required to be at least '{MinimumAge}' but the following exception was triggered while processing one of your age claims: {ex.Message}",
                    new CookieOptions() { MaxAge = TimeSpan.FromSeconds(10) });
                }
              
                context.Fail();
                // temporary fix to see what happens when the age claim is wrong
                Console.WriteLine(ex.ToString());
                return Task.CompletedTask;
            }
        }

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }


    }
}
