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
    public class UserController : Controller
    {
        private  readonly MilLitDbContext _context;

        public UserController(MilLitDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["PublishedBooks"] = await _context.Books.Where(b=>b.CreatedBy == user.UserId).ToListAsync();
            ViewData["LikedBooks"] = await _context.Likes.Where(l=>l.UserId == user.UserId).ToListAsync();
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,PasswordHash,Login,IsAdmin,ProfilePicture")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.UtcNow;
                // Console.WriteLine("CreatedAt: " + user.CreatedAt);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,PasswordHash,Login,IsAdmin,CreatedAt,ProfilePicture")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["PublishedBooks"] = await _context.Books.Where(b=>b.CreatedBy == user.UserId).ToListAsync();
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                var UserBooks = _context.Books.Where(book => book.CreatedBy == user.UserId);
                var UserTags = _context.Tags.Where(tag => tag.CreatedBy == user.UserId);
                var UserComments = _context.Comments.Where(com => com.UserId == user.UserId);
                foreach(var book in UserBooks)
                {
                    book.CreatedBy = null;
                    _context.Update(book);
                }
                foreach(var tag in UserTags)
                {
                    tag.CreatedBy = null;
                    _context.Update(tag);
                }
                
                foreach(var comment in UserComments)
                {
                    comment.UserId = null;
                    _context.Update(comment);
                }

                var LikeList = await _context.Likes.Where(like => like.UserId == user.UserId).ToListAsync();
                foreach(var like in LikeList)
                {
                    _context.Likes.Remove(like);
                }

                var SaveList = await _context.Saves.Where(save => save.UserId == user.UserId).ToListAsync();
                foreach(var save in SaveList)
                {
                    _context.Saves.Remove(save);
                }

                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
