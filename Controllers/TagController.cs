using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIL_LIT;

namespace MIL_LIT.Controllers_
{
    public class TagController : Controller
    {
        private readonly MilLitDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TagController(MilLitDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Tag
        [ActionName("Index")]
        public async Task<IActionResult> Index(string SearchString)
        {
            var milLitDbContext = _context.Tags.Include(t => t.CreatedByNavigation).Include(t => t.ParentTag);
            if(!string.IsNullOrEmpty(SearchString))
            {
                return RedirectToAction("Index for Tag", "Tag", new{SearchString=SearchString});
            }

            return View(await milLitDbContext.ToListAsync());
        }

        [ActionName("Index for Tag")]
        public async Task<IActionResult> Index(int? Id, string SearchString)
        {
            if(!string.IsNullOrEmpty(SearchString))
            {
                var Tags = _context.Tags.Where(t => t.Name.Contains(SearchString)).Include(t => t.CreatedByNavigation).Include(t => t.ParentTag);
                var Books = _context.Books.Where(book => book.GeneralInfo.Contains(SearchString) 
                                                    || book.Author.Contains(SearchString) 
                                                    || book.CreatedByNavigation.Login.Contains(SearchString) 
                                                    || book.Name.Contains(SearchString)).Include(b => b.CreatedByNavigation);
                ViewData["Tags"] = Tags;
                ViewData["Books"] = Books;
            } else
            if(Id is null)
            {
                return RedirectToAction("Index", "Tag");
            } else
            {
                var SubTags = _context.Tags.Where(t => t.ParentTagId == Id).Include(t => t.CreatedByNavigation).Include(t => t.ParentTag);
                var TagBooksList = _context.BookTags.Where(t => t.TagId == Id).Select(t => t.BookId);
                var TagBooks = _context.Books.Where(t => TagBooksList.Contains(t.BookId)).Include(b => b.CreatedByNavigation);
                ViewData["Tags"] = SubTags;
                ViewData["Books"] = TagBooks;
            }
            return View();
        }

        // GET: Tag/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.ParentTag)
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index for Tag", "Tag", new{Id = tag.TagId});
        }

        // GET: Tag/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users.Where(t => t.IsAdmin), "Id", "Login");
            ViewData["ParentTagId"] = new SelectList(_context.Tags, "TagId", "Name");
            return View();
        }

        // POST: Tag/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagId,Name,CreatedBy,CoverImage,ParentTagId,CoverFile")] Tag tag)
        {
            if(_context.Tags.Any(t => t.Name == tag.Name && t.TagId!=tag.TagId))
            {
                ModelState.AddModelError(nameof(tag.Name), "Категорія з такою назвою уже існує.");
            } else
            if (ModelState.IsValid)
            {
                if(tag.CoverFile != null)
                {
                    string folder = "tags/covers/";
                    string FileNameWithoutSpaces = string.Join("", tag.CoverFile.FileName.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                    folder +=  Guid.NewGuid().ToString() + "_" + FileNameWithoutSpaces;
                    string ServerFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    await tag.CoverFile.CopyToAsync(new FileStream(ServerFolder, FileMode.Create));
                    tag.CoverImage = "/"+folder;
                }
                tag.CreatedAt = DateTime.UtcNow;
                _context.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users.Where(t => t.IsAdmin), "Id", "Login", tag.CreatedBy);
            ViewData["ParentTagId"] = new SelectList(_context.Tags, "TagId", "Name", tag.ParentTagId);
            return View(tag);
        }

        // GET: Tag/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users.Where(t => t.IsAdmin), "Id", "Login", tag.CreatedBy);
            ViewData["ParentTagId"] = new SelectList(_context.Tags, "TagId", "Name", tag.ParentTagId);
            return View(tag);
        }

        // POST: Tag/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagId,Name,CreatedBy,CoverImage,CreatedAt,ParentTagId,CoverFile")] Tag tag)
        {
            if (id != tag.TagId)
            {
                return NotFound();
            }

            if(_context.Tags.Any(t => t.Name == tag.Name && t.TagId!=tag.TagId))
            {
                ModelState.AddModelError(nameof(tag.Name), "Категорія з такою назвою уже існує.");
            } else
            if (ModelState.IsValid)
            {
                try
                {
                    if(tag.CoverFile != null)
                    {
                        string folder = "tags/covers/";
                        string FileNameWithoutSpaces = string.Join("", tag.CoverFile.FileName.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                        folder +=  Guid.NewGuid().ToString() + "_" + FileNameWithoutSpaces;
                        string ServerFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                        await tag.CoverFile.CopyToAsync(new FileStream(ServerFolder, FileMode.Create));
                        tag.CoverImage = "/"+folder;
                    }
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.TagId))
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
            ViewData["CreatedBy"] = new SelectList(_context.Users/*.Where(t => t.IsAdmin)*/, "Id", "Login", tag.CreatedBy);
            ViewData["ParentTagId"] = new SelectList(_context.Tags, "TagId", "Name", tag.ParentTagId);
            return View(tag);
        }

        // GET: Tag/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.ParentTag)
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tag/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                var TagBookTags = await _context.BookTags.Where(booktag => booktag.TagId == tag.TagId).ToListAsync();

                var BooksInBookTags = await _context.Books.Where(book => TagBookTags.Select(t => t.BookId).ToList().Contains(book.BookId)).Include(t => t.CreatedByNavigation).ToListAsync();

                foreach(var booktag in TagBookTags)
                {
                    _context.BookTags.Remove(booktag);
                }
                
                var TagList = await _context.Tags.Where(t => t.ParentTagId == tag.TagId).Include(t => t.CreatedByNavigation).Include(t => t.ParentTag).ToListAsync();
                foreach(var childTag in TagList)
                {
                    childTag.ParentTag = null;
                    _context.Tags.Update(childTag);
                    await DeleteConfirmed(childTag.TagId);
                }

                // foreach(var book in BooksInBookTags)
                // {
                //     var booktags = await _context.BookTags.FirstOrDefaultAsync(booktag => (booktag.BookId == book.BookId && booktag.TagId != tag.TagId));
                //     if(booktags == null)
                //     {
                //         _context.Books.Remove(book);
                //     }
                // }
                _context.Tags.Remove(tag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }
    }
}
