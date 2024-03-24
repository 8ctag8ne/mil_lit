using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using MIL_LIT;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace MIL_LIT.Controllers_
{
    public class BookController : Controller
    {
        private readonly MilLitDbContext _context;
        public BookController(MilLitDbContext context)
        {
            _context = context;
        }

        // GET: Book
        public async Task<IActionResult> Index(string SearchString)
        {
            var milLitDbContext = _context.Books.Include(b => b.CreatedByNavigation);

            if(!string.IsNullOrEmpty(SearchString))
            {
                milLitDbContext = milLitDbContext.Where(book => book.GeneralInfo.Contains(SearchString) 
                                                    || book.Author.Contains(SearchString) 
                                                    || book.CreatedByNavigation.Login.Contains(SearchString) 
                                                    || book.Name.Contains(SearchString)).Include(b => b.CreatedByNavigation);
            }
            return View(await milLitDbContext.ToListAsync());
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            var TagList = await _context.BookTags.Where(b => b.BookId == book.BookId).Select(t=>t.TagId).ToListAsync();
            var Tags = _context.Tags.Where(t => TagList.Contains(t.TagId)).Include(t => t.CreatedByNavigation).Include(t => t.ParentTag);
            ViewData["Tags"] = Tags;

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users.Where(user=>user.IsAdmin), "UserId", "Login");
            ViewData["AllTags"] = new MultiSelectList(_context.Tags, "TagId", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,BookId,CreatedBy,SourceLink,Filepath,GeneralInfo,Author,CoverLink, TagIds")] Book book)
        {
            bool ok = true;
            if(_context.Books.Any(b=>b.Name == book.Name && b.BookId!=book.BookId))
            {
                ModelState.AddModelError(nameof(book.Name), "Книга з такою назвою уже існує.");
                ok = false;
            }
            if(_context.Books.Any(b=>b.SourceLink == book.SourceLink && b.BookId!=book.BookId))
            {
                ModelState.AddModelError(nameof(book.SourceLink), "Книга з таким джерелом уже існує.");
                ok = false;
            }
            if (ok && ModelState.IsValid)
            {
                book.TagIds = book.TagIds.Distinct().ToList();
                book.Likes = 0;
                book.Saves = 0;
                book.CreatedAt = DateTime.UtcNow;
                foreach(var TagId in book.TagIds)
                {
                    var tag = await _context.Tags.FindAsync(TagId);
                    BookTag bookTag = new BookTag();
                    bookTag.BookId = book.BookId;
                    bookTag.TagId = TagId;
                    bookTag.Book = book;
                    bookTag.Tag = await _context.Tags.FirstOrDefaultAsync(m => m.TagId == TagId);
                    _context.BookTags.Add(bookTag);
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", book.CreatedBy);
            ViewData["AllTags"] = new MultiSelectList(_context.Tags, "TagId", "Name");
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            int[] tags = await _context.BookTags.Where(bt=>bt.BookId == book.BookId).Select(t=>t.TagId).ToArrayAsync();
            book.TagIds = tags.ToList();
            ViewData["CreatedBy"] = new SelectList(_context.Users.Where(user=>user.IsAdmin), "UserId", "Login", book.CreatedBy);
            ViewData["AllTags"] = new MultiSelectList(_context.Tags, "TagId", "Name", tags);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,BookId,CreatedBy,Likes,Saves,CreatedAt,SourceLink,Filepath,GeneralInfo,Author,CoverLink,TagIds")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            bool ok = true;
            if(_context.Books.Any(b=>b.Name == book.Name && b.BookId!=book.BookId))
            {
                ModelState.AddModelError(nameof(book.Name), "Книга з такою назвою уже існує.");
                ok = false;
            }
            if(_context.Books.Any(b=>b.SourceLink == book.SourceLink && b.BookId!=book.BookId))
            {
                ModelState.AddModelError(nameof(book.SourceLink), "Книга з таким джерелом уже існує.");
                ok = false;
            }
            if (ok && ModelState.IsValid)
            {
                try
                {
                    var TagList = await _context.BookTags.Where(tag => tag.BookId == book.BookId).ToListAsync();
                    foreach(var booktag in TagList)
                    {
                        _context.BookTags.Remove(booktag);
                    }
                    foreach(var TagId in book.TagIds)
                    {
                        var tag = await _context.Tags.FindAsync(TagId);
                        BookTag bookTag = new BookTag();
                        bookTag.BookId = book.BookId;
                        bookTag.TagId = TagId;
                        bookTag.Book = book;
                        bookTag.Tag = await _context.Tags.FirstOrDefaultAsync(m => m.TagId == TagId);
                        _context.BookTags.Add(bookTag);
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            int[] tags = await _context.BookTags.Where(bt=>bt.BookId == book.BookId).Select(t=>t.TagId).ToArrayAsync();
            ViewData["CreatedBy"] = new SelectList(_context.Users, "UserId", "Login", book.CreatedBy);
            ViewData["AllTags"] = new MultiSelectList(_context.Tags, "TagId", "Name", tags);
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            var TagList = await _context.BookTags.Where(b => b.BookId == book.BookId).Select(t=>t.TagId).ToListAsync();
            var Tags = _context.Tags.Where(t => TagList.Contains(t.TagId)).Include(t => t.CreatedByNavigation).Include(t => t.ParentTag);
            ViewData["Tags"] = Tags;

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                var TagList = await _context.BookTags.Where(tag => tag.BookId == book.BookId).ToListAsync();
                foreach(var booktag in TagList)
                {
                    _context.BookTags.Remove(booktag);
                }

                var LikeList = await _context.Likes.Where(like => like.BookId == book.BookId).ToListAsync();
                foreach(var like in LikeList)
                {
                    _context.Likes.Remove(like);
                }

                var SaveList = await _context.Saves.Where(save => save.BookId == book.BookId).ToListAsync();
                foreach(var save in SaveList)
                {
                    _context.Saves.Remove(save);
                }
                
                var comments = await _context.Comments.Where(c=>c.BookId==book.BookId).ToListAsync();
                foreach(var comment in comments)
                {
                    _context.Comments.Remove(comment);
                }
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
