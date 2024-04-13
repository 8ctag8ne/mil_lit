using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIL_LIT;

namespace MIL_LIT.Controllers_
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly MilLitDbContext _context;

        public CommentController(MilLitDbContext context)
        {
            _context = context;
        }

        // GET: Comment
        public async Task<IActionResult> Index()
        {
            var milLitDbContext = _context.Comments.Include(c => c.Book).Include(c => c.ParentComment).Include(c => c.User);
            return View(await milLitDbContext.ToListAsync());
        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Book)
                .Include(c => c.ParentComment)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comment/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name");
            ViewData["ParentCommentId"] = new SelectList(_context.Comments, "CommentId", "CommentId");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login");
            return View();
        }

        // POST: Comment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Text,UserId,BookId,PostedAt,ParentCommentId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Book", new {id = comment.BookId});
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name", comment.BookId);
            ViewData["ParentCommentId"] = new SelectList(_context.Comments, "CommentId", "CommentId", comment.ParentCommentId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", comment.UserId);
            return View(comment);
        }

        // GET: Comment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }


            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", comment.BookId);
            ViewData["ParentCommentId"] = new SelectList(_context.Comments, "CommentId", "CommentId", comment.ParentCommentId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", comment.UserId);
            if(Convert.ToString(comment.UserId) == User.FindFirstValue(ClaimTypes.NameIdentifier) || 
            User.IsInRole("admin") || 
            User.IsInRole("moderator"))
            {
                return View(comment);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,Text,UserId,BookId,PostedAt,ParentCommentId")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if(Convert.ToString(comment.UserId) != User.FindFirstValue(ClaimTypes.NameIdentifier) && 
            !User.IsInRole("admin") && 
            !User.IsInRole("moderator"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
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
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", comment.BookId);
            ViewData["ParentCommentId"] = new SelectList(_context.Comments, "CommentId", "CommentId", comment.ParentCommentId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Login", comment.UserId);
            return View(comment);
        }

        // GET: Comment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Book)
                .Include(c => c.ParentComment)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            if(Convert.ToString(comment.UserId) == User.FindFirstValue(ClaimTypes.NameIdentifier) || 
            User.IsInRole("admin") || 
            User.IsInRole("moderator"))
            {
                return View(comment);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if(Convert.ToString(comment?.UserId) != User.FindFirstValue(ClaimTypes.NameIdentifier) && 
            !User.IsInRole("admin") && 
            !User.IsInRole("moderator"))
            {
                return RedirectToAction("Index", "Home");
            }

            
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
