using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Chirp.Core.Interfaces;
using Chirp.Core.Domain;
using Chirp.Core.Services;
using Xunit;

namespace Chirp.Web;
public class Tests
{
	//Tests for the database
	/*
	
	[Fact]
	public void UnitUserCreatingTest()
	{
		var userId = 12345;
		var username = "Tester";
		var email = "Tester@email.com";

		var user = new Author
		{
			AuthorId = userId,
			Name = username,
			Email = email
		};

		Assert.Equal(userId, user.AuthorId);
		Assert.Equal(username, user.Name);
		Assert.Equal(email, user.Email);
	}

	[Fact]
	public void UnitMessageCreatingTest()
	{
		var user = new Author
		{
			AuthorId = 12345,
			Name = "Tester",
			Email = "Tester@email.com"
		};

		var sentAt = DateTime.UtcNow;

		var message = new Cheep
		{
			CheepId = 99999,
			Text = "Jeg hopper fra femte!",
			TimeStamp = sentAt,
			AuthorId = user.AuthorId,
			Author = user
		};

		Assert.Equal(99999, message.CheepId);
		Assert.Equal("Jeg hopper fra femte!", message.Text);
		Assert.Equal(user.AuthorId, message.AuthorId);
		Assert.Equal(user, message.Author);
		Assert.Equal(sentAt, message.TimeStamp);
	}

	[Fact]
	public void IntegrationMessageDataBaseTest()
	{
		var options = new DbContextOptionsBuilder<ChatDBContext>()
			.UseSqlite("Data Source=:memory:")
			.Options;

		using var context = new ChatDBContext(options);
		context.Database.OpenConnection();
		context.Database.EnsureCreated();

		var a1 = new Author()
		{
			AuthorId = 1,
			Name = "Roger Histand",
			Email = "Roger+Histand@hotmail.com",
			Cheeps = new List<Cheep>()
		};

		var c1 = new Cheep()
		{
			CheepId = 13,
			AuthorId = a1.AuthorId,
			Author = a1,
			Text = "You are here for at all?",
			TimeStamp = DateTimeOffset.FromUnixTimeSeconds(1690895598).UtcDateTime
		};

		context.Authors.Add(a1);
		context.Cheeps.Add(c1);
		context.SaveChanges();

		var facade = new CheepRepository(context);
		var cheeps = facade.GetCheeps();

		DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(1690895598);
		string formatted = dateTime.UtcDateTime.ToString("MM/dd/yy H:mm:ss");

		Assert.Single(cheeps);
		Assert.Equal("Roger Histand", cheeps[0].Author);
		Assert.Equal("You are here for at all?", cheeps[0].Message);
		Assert.Equal(formatted, cheeps[0].Timestamp);
	}




	[Fact]
	public void GetAuthorFromNameUnitTest()
	{
		//Act
		var options = new DbContextOptionsBuilder<ChatDBContext>()
				.UseSqlite("Data Source=:memory:")
			   .Options;

		using var context = new ChatDBContext(options);
		context.Database.OpenConnection();
		context.Database.EnsureCreated();

		var author = new Author { AuthorId = 1, Name = "Test", Email = "test@test.com" };
		context.Authors.Add(author);
		context.SaveChanges();

		var repo = new AuthorRepository(context);
		var result = repo.GetAuthorFromName("Test");

		//Assert
		Assert.NotNull(result);
		Assert.Equal("Test", result.Name);
		Assert.Equal("test@test.com", result.Email);
	}

	[Fact]
	public void GetAuthorFromEmailUnitTest()
	{
		//Act
		var options = new DbContextOptionsBuilder<ChatDBContext>()
				.UseSqlite("Data Source=:memory:")
			   .Options;

		using var context = new ChatDBContext(options);
		context.Database.OpenConnection();
		context.Database.EnsureCreated();

		var author = new Author { AuthorId = 1, Name = "Test", Email = "test@test.com" };
		context.Authors.Add(author);
		context.SaveChanges();

		var repo = new AuthorRepository(context);
		var result = repo.GetAuthorFromEmail("test@test.com");

		//Assert
		Assert.NotNull(result);
		Assert.Equal("Test", result.Name);
		Assert.Equal("test@test.com", result.Email);
	}
	
	[Fact]
	public void CreateCheep_AddsCheepWithAuthor()
	{
		//Act
    	var options = new DbContextOptionsBuilder<ChatDBContext>()
        .UseSqlite("Data Source=:memory:")
        .Options;

    	using var context = new ChatDBContext(options);
    	context.Database.OpenConnection();
    	context.Database.EnsureCreated();

    	var author = new Author { Name = "Test", Email = "test@test.com" };
    	context.Authors.Add(author);
    	context.SaveChanges();

    	var repo = new CheepRepository(context);
    	repo.CreateCheep("This is a test!", author);

		//Assert
		var cheep = context.Cheeps.Include(c => c.Author).FirstOrDefault();
    	Assert.NotNull(cheep);
    	Assert.Equal("This is a test!", cheep.Text);
    	Assert.Equal(author.Name, cheep.Author.Name);
	}
	[Fact]
	public void IntegrationMessageByUserDataBaseTest()
	{
		var options = new DbContextOptionsBuilder<ChatDBContext>()
			.UseSqlite("Data Source=:memory:")
			.Options;

		using var context = new ChatDBContext(options);
		context.Database.OpenConnection();
		context.Database.EnsureCreated();

		var a1 = new Author()
		{
			AuthorId = 1,
			Name = "Roger Histand",
			Email = "Roger+Histand@hotmail.com",
			Cheeps = new List<Cheep>()
		};

		var a2 = new Author()
		{
			AuthorId = 2,
			Name = "Luanna Muro",
			Email = "Luanna-Muro@ku.dk",
			Cheeps = new List<Cheep>()
		};

		var c1 = new Cheep()
		{
			CheepId = 13,
			AuthorId = a1.AuthorId,
			Author = a1,
			Text = "You are here for at all?",
			TimeStamp = DateTimeOffset.FromUnixTimeSeconds(1690895598).UtcDateTime
		};

		var c2 = new Cheep()
		{
			CheepId = 7,
			AuthorId = a2.AuthorId,
			Author = a2,
			Text = "It was but a very ancient cluster of blocks generally painted green, and for no other, he shielded me.",
			TimeStamp = DateTimeOffset.FromUnixTimeSeconds(1690895641).UtcDateTime
		};

		var c3 = new Cheep()
		{
			CheepId = 56,
			AuthorId = a2.AuthorId,
			Author = a2,
			Text = "See how that murderer could be from any trivial business not connected with her.",
			TimeStamp = DateTimeOffset.FromUnixTimeSeconds(1690895601).UtcDateTime
		};

		context.Authors.Add(a1);
		context.Authors.Add(a2);
		context.Cheeps.Add(c1);
		context.Cheeps.Add(c2);
		context.Cheeps.Add(c3);
		context.SaveChanges();

		var db = new CheepRepository(context);
		var service = new CheepService(db);

		var cheeps = service.GetCheepsFromAuthor("Luanna Muro");

		Assert.Equal(2, cheeps.Count);
		Assert.Equal("Luanna Muro", cheeps[0].Author);
		Assert.Equal("Luanna Muro", cheeps[1].Author);
		Assert.Equal("It was but a very ancient cluster of blocks generally painted green, and for no other, he shielded me.", cheeps[0].Message);
		Assert.Equal("See how that murderer could be from any trivial business not connected with her.", cheeps[1].Message);
	}
	*/
	[Fact]
	public void AuthorCanFollowAnotherAuthor()
	{
		var options = new DbContextOptionsBuilder<ChatDBContext>()
			.UseInMemoryDatabase("FollowTestDb")
			.Options;

		using var context = new ChatDBContext(options);

		var alice = new Author { Id = 1, Name = "Alice", Email = "alice@test.com" };
		var bob = new Author { Id = 2, Name = "Bob", Email = "bob@test.com" };
		context.Authors.Add(alice);
		context.Authors.Add(bob);
		context.SaveChanges();

		context.Follows.Add(new Follow { FollowerId = alice.Id, FollowedId = bob.Id });
		context.SaveChanges();

		var follow = context.Follows.FirstOrDefault(f => f.FollowerId == alice.Id && f.FollowedId == bob.Id);
		Assert.NotNull(follow);
	}
	
	[Fact]
	public void AuthorCannotFollowThemselves()
	{
		var options = new DbContextOptionsBuilder<ChatDBContext>()
			.UseInMemoryDatabase("FollowTestDb2")
			.Options;

		using var context = new ChatDBContext(options);

		var alice = new Author { Id = 1, Name = "Alice", Email = "alice@test.com" };
		context.Authors.Add(alice);
		context.SaveChanges();

		context.Follows.Add(new Follow { FollowerId = alice.Id, FollowedId = alice.Id });

		context.SaveChanges();

		var follow = context.Follows.FirstOrDefault(f => f.FollowerId == alice.Id && f.FollowedId == alice.Id);
		Assert.NotNull(follow); 
	}

	
	[Fact]
	public void AuthorCanFollowMultipleUsers()
	{
		var options = new DbContextOptionsBuilder<ChatDBContext>()
			.UseInMemoryDatabase("FollowTestDb3")
			.Options;

		using var context = new ChatDBContext(options);

		var alice = new Author { Id = 1, Name = "Alice", Email = "alice@test.com" };
		var bob = new Author { Id = 2, Name = "Bob", Email = "bob@test.com" };
		var charlie = new Author { Id = 3, Name = "Charlie", Email = "charlie@test.com" };

		context.Authors.AddRange(alice, bob, charlie);
		context.SaveChanges();

		context.Follows.Add(new Follow { FollowerId = alice.Id, FollowedId = bob.Id });
		context.Follows.Add(new Follow { FollowerId = alice.Id, FollowedId = charlie.Id });
		context.SaveChanges();

		var follows = context.Follows.Where(f => f.FollowerId == alice.Id).ToList();
		Assert.Equal(2, follows.Count);
		Assert.Contains(follows, f => f.FollowedId == bob.Id);
		Assert.Contains(follows, f => f.FollowedId == charlie.Id);
	}


}