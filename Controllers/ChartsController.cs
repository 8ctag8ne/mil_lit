using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MIL_LIT.Controllers_;
[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    private record CountByAuthorResponseItem(string Author, int Count);
    private readonly MilLitDbContext _context;
    public ChartsController(MilLitDbContext _context)
    {
        this._context = _context;
    }
    [HttpGet("countByAuthor")]
    public async Task<JsonResult>
    GetCountByAuthorAsync(CancellationToken cancellationToken)
    {
    var responseItems = await _context.Books
                                           .GroupBy(book => book.Author).OrderByDescending(book=>book.Count())
                                           .Select(group => new
    CountByAuthorResponseItem(group.Key.ToString(), group.Count())).ToListAsync(cancellationToken);

    var items = responseItems[0..Math.Min(5, responseItems.Count())];
    return new JsonResult(items);
    }
}