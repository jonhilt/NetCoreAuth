using NetCoreJWTAuth.App;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;
using Microsoft.AspNetCore.Authorization;

namespace NetCoreJWTAuth.Tests
{
    public class MinimumEmploymentMonthTests
    {
        [Fact]
        public void EmployeeWithRequiredMonthsServiceIsAuthorized()
        {
            var employmentCommenced = DateTime.Now.AddMonths(-3).AddDays(-1).ToString();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                new Claim(CustomClaimTypes.EmploymentCommenced, employmentCommenced, ClaimValueTypes.DateTime)
            }));

            var requirement = new MinimumMonthsEmployedRequirement(3);
            var authorizationHandler = new MinimumMonthsEmployedHandler();

            var handlers = new List<IAuthorizationRequirement> { requirement };
            var context = new AuthorizationHandlerContext(handlers, user, null);
            authorizationHandler.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public void EmployeeWithoutRequiredMonthsOfServiceDenied()
        {
            var employmentCommenced = DateTime.Now.AddMonths(-2).ToString();           

            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {
                new Claim(CustomClaimTypes.EmploymentCommenced, DateTime.Now.AddMonths(-2).ToString(), ClaimValueTypes.DateTime)
            }));

            var requirement = new MinimumMonthsEmployedRequirement(3);
            var authorizationHandler = new MinimumMonthsEmployedHandler();

            var handlers = new List<IAuthorizationRequirement> { requirement };
            var context = new AuthorizationHandlerContext(handlers, user, null);
            authorizationHandler.HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }
    }
}
