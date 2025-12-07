using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Chirp.Core.Domain;

public static class TestHelpers
{
    public static Mock<UserManager<Author>> MockUserManager(Author user)
    {
        var store = new Mock<IUserStore<Author>>();

        var mgr = new Mock<UserManager<Author>>(
            store.Object, null, null, null, null, null, null, null, null
        );

        mgr.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        mgr.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns(user.Id.ToString());

        return mgr;
    }

    public static PageContext CreatePageContextWithUser(string username)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "TestAuth")
        );

        return new PageContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData()
        };
    }
}
