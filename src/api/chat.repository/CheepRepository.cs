namespace MyChat.Razor.chat.repository;
using Microsoft.EntityFrameworkCore;
using MyChat.Razor.data;


public class CheepRepository : iCheepRepository
{

    private readonly ChatDBContext _context;

    public CheepRepository(ChatDBContext context)
    {
        _context = context;
    }
    public List<CheepViewModel> GetCheeps(int pageNumber = 1)
    {
        int limit = 32;
        int offset = (pageNumber - 1) * limit;

        return _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(offset)
            .Take(limit)
            .Select(c => new CheepViewModel(
                c.Author.Name,
                c.Text,
                c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            ))
            .ToList();
    }
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1) {
        int limit = 32;
        int offset = (pageNumber - 1) * limit;

        return _context.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(offset)
            .Take(limit)
            .Select(c => new CheepViewModel(
                c.Author.Name,
                c.Text,
                c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            ))
            .ToList();
    }
}