using Microsoft.AspNetCore.Http;
using Moq;
using Photobook.Common.Services.CurrentUser;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Photobook.Tests.Common.Services.CurrentUser
{
    public class CurrentUserServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly CurrentUserService _classUnderTest;

        public CurrentUserServiceTests()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _classUnderTest = new CurrentUserService(_httpContextAccessor.Object);
        }

        [Fact]
        public void GetUserId_ShouldReturnUserId_WhenSucceed()
        {
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("sub", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var claimPrincipal = new ClaimsPrincipal(identity);
            _httpContextAccessor.Setup(x => x.HttpContext.User)
                .Returns(claimPrincipal);

            var result = _classUnderTest.GetUserId();

            Assert.Equal(userId, result);
        }

        [Fact]
        public void GetUserId_ShouldReturnNull_WhenClaimSubNotExists()
        {
            var claimPrincipal = new ClaimsPrincipal();
            _httpContextAccessor.Setup(x => x.HttpContext.User)
                .Returns(claimPrincipal);

            var result = _classUnderTest.GetUserId();

            Assert.Null(result);
        }
    }
}
