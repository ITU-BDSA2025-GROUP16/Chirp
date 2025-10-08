using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;

namespace MyChat.Razor.Model;


using Xunit;
public class Tests
{
	[Fact]
	public void UnitUserCreatingTest()
	{

		var userId = 12345;
		var username = "Tester";
		var email = "Tester@email.com";
		
		var user = new User
		{
			Id = userId,
			Username = username,
			Email = email
		};
	
		Assert.Equal(user.Id, userId);
		Assert.Equal(user.Username, username);
		Assert.Equal(user.Email, email);
		
		Assert.Equal(user.Id, 12345);
		Assert.Equal(user.Username, "Tester");
		Assert.Equal(user.Email, "Tester@email.com");
	}
	
	[Fact]
	public void UnitMessageCreatingTest()
	{

		var userId = 12345;
		var username = "Tester";
		var email = "Tester@email.com";
		
		var user = new User
		{
			Id = userId,
			Username = username,
			Email = email
		};

		var id = 99999;
		var content = "Jeg hopper fra femte!";
		var sentAt = DateTime.Now;
		
		var message = new Message
		{
			Id = id,
			Content = content,
			SentAt = sentAt, 
			UserId = userId,
			User = user
		};
		
		
		
		Assert.Equal(message.Id, id);
		Assert.Equal(message.Content, content);
		Assert.Equal(message.SentAt, sentAt);
		Assert.Equal(message.UserId, userId);
		Assert.Equal(message.User, user);
		
		Assert.Equal(message.Id, 99999);
		Assert.Equal(message.Content, "Jeg hopper fra femte!");
		Assert.Equal(message.UserId, 12345);
	}
}