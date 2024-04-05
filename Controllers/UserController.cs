using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIL_LIT;

namespace MIL_LIT.Controllers_
{
    public class UserController : Controller
    {
        private  readonly MilLitDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserController(MilLitDbContext context, IWebHostEnvironment webHostEnvironment, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
            _userManager = userManager;
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["PublishedBooks"] = await _context.Books.Where(b=>b.CreatedBy == user.Id).ToListAsync();
            ViewData["LikedBooks"] = await _context.Likes.Where(l=>l.UserId == user.Id).ToListAsync();
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
        public async Task<IActionResult> Create([Bind("Id,PasswordHash,Login,IsAdmin,ProfilePicture,CoverFile")] User user)
        {
            if (ModelState.IsValid)
            {
                if(user.CoverFile != null)
                {
                    string folder = "users/covers/";
                    string FileNameWithoutSpaces = string.Join("", user.CoverFile.FileName.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                    folder +=  Guid.NewGuid().ToString() + "_" + FileNameWithoutSpaces;
                    string ServerFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    await user.CoverFile.CopyToAsync(new FileStream(ServerFolder, FileMode.Create));
                    user.ProfilePicture = "/"+folder;
                }
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

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,PasswordHash,Login,IsAdmin,CreatedAt,ProfilePicture,CoverFile")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(user.CoverFile != null)
                    {
                        string folder = "users/covers/";
                        string FileNameWithoutSpaces = string.Join("", user.CoverFile.FileName.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                        folder +=  Guid.NewGuid().ToString() + "_" + FileNameWithoutSpaces;
                        string ServerFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                        await user.CoverFile.CopyToAsync(new FileStream(ServerFolder, FileMode.Create));
                        user.ProfilePicture = "/"+folder;
                    }
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    await _userManager.UpdateAsync(user);
                    // _context.Update(user);
                    // await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["PublishedBooks"] = await _context.Books.Where(b=>b.CreatedBy == user.Id).ToListAsync();
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            var CurrentUser = _userManager.GetUserAsync(User);
            if (user != null)
            {
                var UserBooks = _context.Books.Where(book => book.CreatedBy == user.Id);
                var UserTags = _context.Tags.Where(tag => tag.CreatedBy == user.Id);
                var UserComments = _context.Comments.Where(com => com.UserId == user.Id);
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

                var LikeList = await _context.Likes.Where(like => like.UserId == user.Id).ToListAsync();
                foreach(var like in LikeList)
                {
                    _context.Likes.Remove(like);
                }

                var SaveList = await _context.Saves.Where(save => save.UserId == user.Id).ToListAsync();
                foreach(var save in SaveList)
                {
                    _context.Saves.Remove(save);
                }

                _context.Users.Remove(user);
                await _userManager.DeleteAsync(user); //?
            }
            await _context.SaveChangesAsync();
            if(user is not null && CurrentUser.Id == user.Id)
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction(nameof(Index)); 
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
