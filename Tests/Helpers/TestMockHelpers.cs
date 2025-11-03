using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Tests.Helpers
{
    public static class TestMockHelpers
    {
        // UserManager<T> has no interface, so we mock it using IUserStore<T>
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object, default!, default!, default!, default!, default!, default!, default!, default!);

        }
        
        public static Mock<SignInManager<TUser>> MockSignInManager<TUser>(Mock<UserManager<TUser>> userManager) where TUser : class
        {
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            return new Mock<SignInManager<TUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, // IOptions<IdentityOptions>
                null, // ILogger<SignInManager<TUser>>
                null, // IAuthenticationSchemeProvider
                null  // IUserConfirmation<TUser>
            );
        }


        public static ApplicationUser RandomUser()
        {
            Random rnd = new Random();
            DateTime start = new DateTime(1950, 1, 1);
            DateTime end = new DateTime(2025, 1, 1);
            
            return new ApplicationUser
            {
                Id = rnd.Next(1, 9999),
                Name = "Test User",
                DateOfBirth = start.AddDays(rnd.Next((end-start).Days))
            };
        }
    }
}
