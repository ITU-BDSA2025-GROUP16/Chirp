using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;
using MyChat.Razor.data;
using MyChat.Razor.Model; 

namespace MyChat.Razor.Tests;


using Xunit;
public class Tests
{
	[Fact]
	public void UnitUserCreatingTest()
	{
		var userId = 12345;
		var username = "Tester";
		var email = "Tester@email.com";

		var user = new Author
		{
			Id = userId,
			Username = username,
			Email = email
		};

		Assert.Equal(userId, user.Id);
		Assert.Equal(username, user.Username);
		Assert.Equal(email, user.Email);
	}

	[Fact]
	public void UnitMessageCreatingTest()
	{
		var user = new Author
		{
			Id = 12345,
			Username = "Tester",
			Email = "Tester@email.com"
		};

		var sentAt = DateTime.UtcNow;

		var message = new Cheep
		{
			Id = 99999,
			Content = "Jeg hopper fra femte!",
			SentAt = sentAt,
			UserId = user.Id,
			Author = user
		};

		Assert.Equal(99999, message.Id);
		Assert.Equal("Jeg hopper fra femte!", message.Content);
		Assert.Equal(user.Id, message.UserId);
		Assert.Equal(user, message.Author);
		Assert.Equal(sentAt, message.SentAt);
	}
	
	[Fact]
	public void IntegrationMessageDataBaseTest() //This test was made with the help of LLM
	{
		string testDbPath = Path.Combine(Path.GetTempPath(), $"testdb_{Guid.NewGuid()}.db");
		
		if (File.Exists(testDbPath))
			File.Delete(testDbPath);
		
		using (var connection = new SqliteConnection($"Data Source={testDbPath}"))
		{
			connection.Open();
			var command = connection.CreateCommand(); //Need to add method for cheeping into db
			command.CommandText = @" 
                CREATE TABLE user (
                    user_id INTEGER PRIMARY KEY,
                    username TEXT,
                    email TEXT,
                    password TEXT
                );
                CREATE TABLE message (
                    message_id INTEGER PRIMARY KEY,
                    author_id INTEGER,
                    text TEXT,
                    pub_date INTEGER
                );
                INSERT INTO user VALUES(1,'Roger Histand','Roger+Histand@hotmail.com','AZ0serCHTCtJWR+sQCF4MhhfYLyLuK9tU4bWVy0AOBU=');
                INSERT INTO message VALUES(13,1,'You are here for at all?',1690895598);
            ";
			command.ExecuteNonQuery();
		}

		DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(1690895598);
		string formatted = dateTime.UtcDateTime.ToString("MM/dd/yy H:mm:ss");
		
		var facade = new DBFacade(testDbPath);
		var cheeps = facade.GetCheeps();

		Assert.Single(cheeps);
		Assert.Equal("Roger Histand", cheeps[0].Author);
		Assert.Equal("You are here for at all?", cheeps[0].Message);
		Assert.Equal(formatted, cheeps[0].Timestamp);
	}
	
	
	[Fact]
	public void IntegrationMessageByUserDataBaseTest()
	{
		var dbPath = Path.Combine(Path.GetTempPath(), $"testdb_{Guid.NewGuid()}.db");
		
		if (File.Exists(dbPath)) 
			File.Delete(dbPath);

		using (var connection = new SqliteConnection($"Data Source={dbPath}"))
		{
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @" 
                CREATE TABLE user (
                    user_id INTEGER PRIMARY KEY,
                    username TEXT,
                    email TEXT,
                    password TEXT
                );
                CREATE TABLE message (
                    message_id INTEGER PRIMARY KEY,
                    author_id INTEGER,
                    text TEXT,
                    pub_date INTEGER
                );
            INSERT INTO user VALUES(1,'Roger Histand','Roger+Histand@hotmail.com','AZ0serCHTCtJWR+sQCF4MhhfYLyLuK9tU4bWVy0AOBU=');
			INSERT INTO user VALUES(2,'Luanna Muro','Luanna-Muro@ku.dk','iuN9GA9kRoMQW54696JGUML74QKYNKFaG9mFNozJzqQ=');
            INSERT INTO message VALUES(13,1,'You are here for at all?',1690895598);
			INSERT INTO message VALUES(7,2,'It was but a very ancient cluster of blocks generally painted green, and for no other, he shielded me.',1690895641);
			INSERT INTO message VALUES(56,2,'See how that murderer could be from any trivial business not connected with her.',1690895601);
        ";
			command.ExecuteNonQuery();
		}

		var db = new DBFacade(dbPath);
		var service = new CheepService(db);

		// Act
		var cheeps = service.GetCheepsFromAuthor("Luanna Muro");

		// Assert
		Assert.Equal(2, cheeps.Count);
		Assert.Equal("Luanna Muro", cheeps[0].Author);
		Assert.Equal("Luanna Muro", cheeps[1].Author);
		Assert.Equal("It was but a very ancient cluster of blocks generally painted green, and for no other, he shielded me.", cheeps[0].Message);
		Assert.Equal("See how that murderer could be from any trivial business not connected with her.", cheeps[1].Message);

	}
}