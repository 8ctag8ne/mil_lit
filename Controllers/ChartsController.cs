using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MIL_LIT.Controllers_;
[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    private record CountByAuthorResponseItem(string Author, int Count);
    private record FavouriteTagResponseItem(string Tag, int Count);
    private readonly MilLitDbContext _context;
    public ChartsController(MilLitDbContext _context)
    {
        this._context = _context;
    }
    [HttpGet("countByAuthor")]
    public async Task<JsonResult> GetCountByAuthorAsync(CancellationToken cancellationToken)
    {
    var responseItems = await _context.Books
                                           .GroupBy(book => book.Author).OrderByDescending(book=>book.Count())
                                           .Select(group => new
    CountByAuthorResponseItem(group.Key.ToString(), group.Count())).ToListAsync(cancellationToken);

    var items = responseItems[0..Math.Min(5, responseItems.Count())];
    return new JsonResult(items);
    }

    [HttpGet("FavouriteTags")]
    public async Task<JsonResult> GetFavouriteTagsAsync(CancellationToken cancellationToken, [FromQuery] int UserId)
    {
        var Likes = await _context.Likes.Where(like => like.UserId == UserId).Select(like => like.BookId).ToListAsync(cancellationToken);
        var tags =  await _context.BookTags.Where(bt => Likes.Contains(bt.BookId))
                                    .Include(bt => bt.Tag)
                                    .GroupBy(bt => bt.Tag.Name)
                                    .OrderByDescending(bt => bt.Count())
                                    .Select(group => new 
                                    FavouriteTagResponseItem(group.Key.ToString(), group.Count())).ToListAsync(cancellationToken);

    var items = tags[0..Math.Min(10, tags.Count())];
    return new JsonResult(items);
    }

    [HttpGet("BiggestTags")]
    public async Task<JsonResult> GetBiggestTagsAsync(CancellationToken cancellationToken)
    {
        var tags =  await _context.BookTags
                                    .Include(bt => bt.Tag)
                                    .GroupBy(bt => bt.Tag.Name)
                                    .OrderByDescending(bt => bt.Count())
                                    .Select(group => new 
                                    FavouriteTagResponseItem(group.Key.ToString(), group.Count())).ToListAsync(cancellationToken);

    var items = tags[0..Math.Min(10, tags.Count())];
    return new JsonResult(items);
    }
}