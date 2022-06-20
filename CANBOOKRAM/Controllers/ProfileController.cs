using CANBOOKRAM.Data;
using CANBOOKRAM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CANBOOKRAM.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);

            var follwers = from i in _context.UserFriends where i.Friend.Id == id select i;
            var following = (from i in _context.UserFriends where i.User == applicationUser && i.Friend.Id == id select i).FirstOrDefault();
            var rates = (from i in _context.UserRatings where i.User.Id == id select i.Rate).ToList();
            var rated = (from i in _context.UserRatings where i.User.Id == id && i.WhoRated.Id == applicationUser.Id select i).FirstOrDefault();

            var user = (from i in _context.Users
                        where i.Id == id
                        select new UserModel
                        {
                            UserId = i.Id,
                            Name = i.Name,
                            Email = i.Email,
                            UserName = i.UserName,
                            PhoneNumber = i.PhoneNumber,
                            ProfilePicture = i.ProfilePicture,
                            BirthDate = i.BirthDate,
                            Gender = i.Gender,
                            City = i.City,
                            IsPrivate = i.IsPrivate,
                            FollowerCount = follwers.Count(),
                            IsFollowing = following != null ? true : false,
                            RelationId = following != null ? following.Id : 0,
                            IsRated = rated != null ? true : false,
                            YourRate = rated != null ? rated.Rate : 0
                        }).FirstOrDefault();

            if (user != null)
            {
                user.AvgRating = CalculateRate.GetRating(rates);

                if (!user.IsPrivate || user.UserId == applicationUser.Id || user.IsFollowing)
                {
                    var posts = (from i in _context.UserPosts.Include("User").Include("Likes") where i.User.Id == id orderby i.TimeStamp descending select i).ToList();
                    user.UserPosts = posts;

                    var top = (from i in _context.UserPosts.Include("User").Include("Likes") where i.User.Id == id orderby i.Likes.Count descending select i).ToList();
                    user.TopPosts = top.Take(5).ToList();
                }
            }

            return View(user);
        }

        public async Task<IActionResult> RefreshPosts(string userId)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var user = (from i in _context.Users where i.Id == userId select i).FirstOrDefault();
            var following = (from i in _context.UserFriends where i.User == applicationUser && i.Friend.Id == userId select i).FirstOrDefault();

            if (!user.IsPrivate || user.Id == applicationUser.Id || following != null)
            {
                var posts = (from i in _context.UserPosts.Include("User").Include("Likes") where i.User.Id == userId orderby i.TimeStamp descending select i).ToList();
                return PartialView("_PostListPartial", posts);
            }
            else
            {
                return PartialView("_PostListPartial", null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(string userId, int rate)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var user = (from i in _context.Users where i.Id == userId select i).FirstOrDefault();

            if (user != null)
            {
                var model = (from i in _context.UserRatings where i.User == user && i.WhoRated == applicationUser select i).FirstOrDefault();

                if (model != null)
                {
                    model.Rate = rate;
                    model.DateTime = DateTime.Now;
                } 
                else
                {
                    model = new UserRating();
                    model.User = user;
                    model.WhoRated = applicationUser;
                    model.Rate = rate;
                    model.DateTime = DateTime.Now;

                    _context.UserRatings.Add(model);
                }

                _context.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest(new { message = "User not found" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRating(string userId)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var rating = (from i in _context.UserRatings where i.User.Id == userId && i.WhoRated == applicationUser select i).FirstOrDefault();

            if (rating != null)
            {
                _context.UserRatings.Remove(rating);
                _context.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Rating not found" });
            }
        }
    }
}
