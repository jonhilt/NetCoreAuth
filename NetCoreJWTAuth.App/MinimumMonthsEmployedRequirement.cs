using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;

namespace NetCoreJWTAuth.App
{
    public class MinimumMonthsEmployedRequirement : IAuthorizationRequirement
    {
        public int MinimumMonthsEmployed { get; private set; }

        public MinimumMonthsEmployedRequirement(int minimumMonths)
        {
            this.MinimumMonthsEmployed = minimumMonths;
        }
    }

    public class MinimumMonthsEmployedHandler 
        : AuthorizationHandler<MinimumMonthsEmployedRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            MinimumMonthsEmployedRequirement requirement)
        {
            var employmentCommenced = context.User
                .FindFirst(claim => claim.Type == CustomClaimTypes.EmploymentCommenced).Value;

            var employmentStarted = Convert.ToDateTime(employmentCommenced);
            var today = LocalDate.FromDateTime(DateTime.Now);

            var monthsPassed = Period
                .Between(employmentStarted.ToLocalDateTime(), today.AtMidnight())
                .Months;

            if (monthsPassed >= requirement.MinimumMonthsEmployed)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public static class CustomClaimTypes
    {
        public const string EmploymentCommenced = "EmploymentCommenced";
    }
}