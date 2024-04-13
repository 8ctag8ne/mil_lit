using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIL_LIT;

namespace MIL_LIT.Controllers_
{
    [Authorize]
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
        [Authorize(Roles = "admin")]
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
            ViewData["LikedBooks"] = await _context.Likes.Where(l=>l.UserId == user.Id).Include(lk => lk.Book).ToListAsync();
            return View(user);
        }

        // GET: User/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,PasswordHash,Login,IsAdmin,ProfilePicture,CoverFile,Email")] User user)
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

                // User user1 = new User { Login = user.Login, 
                //                         PasswordHash = user.PasswordHash, 
                //                         Email = user.Email, 
                //                         UserName = user.Login, 
                //                         IsAdmin = user.IsAdmin,
                //                         CreatedAt = user.CreatedAt,
                //                         ProfilePicture = user.ProfilePicture};
                // Console.WriteLine("CreatedAt: " + user.CreatedAt);
                await _userManager.CreateAsync(user, user.PasswordHash);
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

            var user = await _userManager.FindByIdAsync(Convert.ToString(id));
            if (user == null)
            {
                return NotFound();
            }

            if(Convert.ToString(id) == User.FindFirstValue(ClaimTypes.NameIdentifier) || User.IsInRole("admin"))
            {
                return View(user);
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PasswordHash,Login,IsAdmin,CreatedAt,ProfilePicture,CoverFile,Email,UserName")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            
            if(Convert.ToString(id) != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("admin"))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var OldUser = await _userManager.FindByIdAsync(Convert.ToString(id));
                    if(user.CoverFile != null)
                    {
                        string folder = "users/covers/";
                        string FileNameWithoutSpaces = string.Join("", user.CoverFile.FileName.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                        folder +=  Guid.NewGuid().ToString() + "_" + FileNameWithoutSpaces;
                        string ServerFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                        await user.CoverFile.CopyToAsync(new FileStream(ServerFolder, FileMode.Create));
                        user.ProfilePicture = "/"+folder;
                    }
                    OldUser.PasswordHash = user.PasswordHash;
                    OldUser.Email = user.Email;
                    OldUser.Login = user.Login;
                    OldUser.ProfilePicture = user.ProfilePicture;
                    OldUser.IsAdmin = user.IsAdmin;
                    await _userManager.UpdateAsync(OldUser);
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
                if(User.IsInRole("admin"))
                {
                    return RedirectToAction(nameof(Index));
                } else
                {
                    return RedirectToAction("Details", "User", new {id = id});
                }
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

            if(Convert.ToString(id) == User.FindFirstValue(ClaimTypes.NameIdentifier) || User.IsInRole("admin"))
            {
                ViewData["PublishedBooks"] = await _context.Books.Where(b=>b.CreatedBy == user.Id).ToListAsync();
                return View(user);
            }

            return RedirectToAction("Index", "Home");
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
                if(Convert.ToString(id) != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("admin"))
                {
                    return NotFound();
                }
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
                if(user is not null && CurrentUser.Id == user.Id)
                {
                    await _signInManager.SignOutAsync();
                }
                await _userManager.DeleteAsync(user); //?
            }
            return RedirectToAction(nameof(Index)); 
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
