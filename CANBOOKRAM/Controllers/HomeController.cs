using CANBOOKRAM.Data;
using CANBOOKRAM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CANBOOKRAM.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var follwers = _context.UserFriends.Include("User").Where(i => i.Friend == applicationUser).Select(i => i.User);
            var followings = _context.UserFriends.Include("Friend").Where(i => i.User == applicationUser).Select(i => i.Friend.Id).ToList();
            var posts = _context.UserPosts.Include("User").Include("Likes").Where(i => i.User == applicationUser || followings.Contains(i.User.Id)).OrderByDescending(i => i.TimeStamp).ToList();
            var trending = _context.UserPosts.Include("User").Include("Likes").Where(i => i.User == applicationUser || followings.Contains(i.User.Id)).OrderByDescending(i => i.Likes.Count).ToList();

            var users = from u in _context.Users
                        where u.Id != applicationUser.Id && !followings.Contains(u.Id)
                        select u;

            var model = new HomeModel();
            model.UserPosts = posts;
            model.Users = users.ToList();
            model.PostCount = posts.Where(i => i.User == applicationUser).Count();
            model.FollowerCount = follwers.Count();
            model.FollowingCount = followings.Count;
            model.TrendingPosts = trending.Take(5).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(UserPost model)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);

            if (!String.IsNullOrEmpty(model.Message))
            {
                model.TimeStamp = DateTime.Now;
                model.User = applicationUser;

                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        model.Image = dataStream.ToArray();
                    }
                }

                _context.UserPosts.Add(model);
                _context.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Message is null" });
            }
        }

        public async Task<IActionResult> RefreshPosts()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var followings = _context.UserFriends.Where(i => i.User == applicationUser).Select(i => i.Friend);
            var posts = _context.UserPosts.Include("User").Include("Likes").Where(i => i.User == applicationUser || followings.Contains(i.User)).OrderByDescending(i => i.TimeStamp).ToList();

            return PartialView("_PostListPartial", posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRequest(string id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var friend = _userManager.Users.Where(i => i.Id == id).FirstOrDefault();

            if (friend != null)
            {
                if (friend.IsPrivate)
                {
                    var request = _context.FriendRequests.Where(i => i.User == applicationUser && i.Friend == friend).FirstOrDefault();
                    if (request == null)
                    {
                        var followings = _context.UserFriends.Where(i => i.User == applicationUser && i.Friend == friend).FirstOrDefault();

                        if (followings == null)
                        {
                            var model = new FriendRequest();
                            model.DateTime = DateTime.Now;
                            model.Friend = friend;
                            model.User = applicationUser;
                            model.IsAccept = false;

                            _context.FriendRequests.Add(model);
                            _context.SaveChanges();

                            return Json(new { code = 0, message = "Request sent successfully" });
                        }
                        else
                        {
                            return Json(new { code = 1, message = friend.UserName + " currently is in your followings" });
                        }
                    }
                    else
                    {
                        return Json(new { code = 2, message = "You sent request to this user on " + request.DateTime.ToString("yyyy/MM/dd HH:mm") });
                    }
                } 
                else
                {
                    var following = _context.UserFriends.Where(i => i.Friend == friend && i.User == applicationUser).FirstOrDefault();

                    if (following == null)
                    {
                        var model = new UserFriend();
                        model.Friend = friend;
                        model.User = applicationUser;

                        _context.UserFriends.Add(model);
                        _context.SaveChanges();
                    }

                    return Json(new { code = 0, message = "Follow user successfully" });
                }
            }
            else
            {
                return BadRequest(new { message = "User not found" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(int id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var post = _context.UserPosts.Where(i => i.Id == id).FirstOrDefault();

            if (post != null)
            {
                var like = _context.PostLikes.Where(i => i.Post == post && i.User == applicationUser).FirstOrDefault();
                if (like == null)
                {
                    var model = new PostLike();
                    model.Post = post;
                    model.User = applicationUser;
                    model.DateTime = DateTime.Now;

                    _context.PostLikes.Add(model);
                    _context.SaveChanges();

                    return Json(new { message = "You liked this post" });
                }
                else
                {
                    _context.PostLikes.Remove(like);
                    _context.SaveChanges();

                    return Json(new { message = "You unliked this post" });
                }
            }

            return BadRequest(new { message = "Post not found" });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var post = _context.UserPosts.Include("Likes").Where(i => i.Id == id && i.User == applicationUser).FirstOrDefault();

            if (post != null)
            {
                _context.UserPosts.Remove(post);
                _context.PostLikes.RemoveRange(post.Likes);
                _context.SaveChanges();

                return Json(new { message = "Post removed successfully" });
            }

            return BadRequest(new { message = "Post not found" });
        }

        public async Task<IActionResult> Details(int id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var post = (from i in _context.UserPosts.Include("User") where i.Id == id select i).FirstOrDefault();

            if (post != null)
            {
                return View(post);
            }

            return NotFound();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}