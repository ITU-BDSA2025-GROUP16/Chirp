using Xunit;
using Moq;
using Chirp.Web.Pages;
using Chirp.Core.Domain;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Interfaces; 
using Chirp.Core.Domain;    
using Chirp.Core.Services;   
using Moq;


public class InformationPageTests
{
    [Fact]
    public async Task OnGetAsync_PopulatesUserInfo_Cheeps_And_FollowedAuthors()
    {
        var cheepSvc = new Mock<ICheepService>();
        var followSvc = new Mock<IFollowService>();
        var authorRepo = new Mock<IAuthorRepository>();

        var user = new Author
        {
            UserName = "TestUser",
            Name = "TestName",
            Email = "test@mail.com",
            Id = 1
        };
        var userMgr = TestHelpers.MockUserManager(user);
        cheepSvc.Setup(s => s.GetCheepsFromAuthor("TestUser", 1))
                .Returns(new List<CheepViewModel>());

        followSvc.Setup(s => s.GetFollowedIds(1))
                 .ReturnsAsync(new HashSet<int>());

        var page = new InformationPageModel(
            cheepSvc.Object, 
            followSvc.Object, 
            userMgr.Object, 
            authorRepo.Object
        );

        page.PageContext = TestHelpers.CreatePageContextWithUser("TestUser");
        await page.OnGetAsync();
        Assert.Equal("TestName", page.Name);       
        Assert.Equal("test@mail.com", page.Username); 
        Assert.Equal("test@mail.com", page.Email);        

        Assert.Empty(page.Cheeps);
        Assert.Empty(page.FollowedAuthors);
    }
    [Fact]
    public async Task OnGetAsync_PopulatesCheeps_WhenUserHasCheeps()
    {
        // Arrange
        var author = new Author
        {
            Id = 1,
            UserName = "TestUser",
            Name = "Test Name",
            Email = "test@mail.com"
        };

        var cheeps = new List<CheepViewModel>
        {
            new CheepViewModel(
                "TestUser",      
                "Hello!",        
                "01/01/25 12:00:00", 
                1,               
                1,               
                0                
            )
        };


        var cheepSvc = new Mock<ICheepService>();
        cheepSvc.Setup(s => s.GetCheepsFromAuthor("TestUser", 1)).Returns(cheeps);


        var followSvc = new Mock<IFollowService>();
        followSvc
            .Setup(f => f.GetFollowedIds(It.IsAny<int>()))
            .ReturnsAsync(new HashSet<int> { 2 });

        var authorRepo = new Mock<IAuthorRepository>();
        authorRepo.Setup(r => r.GetAuthorFromId(2))
                .Returns(new Author { Id = 2, Name = "FollowedAuthor", UserName = "followeduser" });

        var userMgr = TestHelpers.MockUserManager(author);

        var page = new InformationPageModel(cheepSvc.Object, followSvc.Object, userMgr.Object, authorRepo.Object);
        page.PageContext = TestHelpers.CreatePageContextWithUser("TestUser");

        await page.OnGetAsync();

        Assert.Single(page.Cheeps);
        Assert.Equal("Hello!", page.Cheeps[0].Message);
        Assert.Equal(author.Name, page.Name);
        Assert.Equal(author.UserName, page.Username);
        Assert.Equal(author.Email, page.Email);

        Assert.Single(page.FollowedAuthors);
        Assert.Equal("FollowedAuthor", page.FollowedAuthors[0].Name);
    }


    [Fact]
    public async Task OnGetAsync_PopulatesFollowedAuthors_WhenUserFollowsOthers()
    {
        var cheepSvc = new Mock<ICheepService>();
        cheepSvc.Setup(s => s.GetCheepsFromAuthor("TestUser", 1)).Returns(new List<CheepViewModel>());

        var followSvc = new Mock<IFollowService>();
        followSvc.Setup(s => s.GetFollowedIds(1)).ReturnsAsync(new HashSet<int> { 2, 3 });

        var userMgr = TestHelpers.MockUserManager(new Author { UserName = "TestUser", Name = "TestName", Email = "test@mail.com", Id = 1 });
        var authorRepo = new Mock<IAuthorRepository>();
        authorRepo.Setup(r => r.GetAuthorFromId(2)).Returns(new Author { Id = 2, Name = "Alice", Email = "alice@test.com" });
        authorRepo.Setup(r => r.GetAuthorFromId(3)).Returns(new Author { Id = 3, Name = "Bob", Email = "bob@test.com" });

        var page = new InformationPageModel(cheepSvc.Object, followSvc.Object, userMgr.Object, authorRepo.Object);
        page.PageContext = TestHelpers.CreatePageContextWithUser("TestUser");

        await page.OnGetAsync();

        Assert.Equal(2, page.FollowedAuthors.Count);
        Assert.Contains(page.FollowedAuthors, a => a.Name == "Alice");
        Assert.Contains(page.FollowedAuthors, a => a.Name == "Bob");
    }

}
