using CANBOOKRAM.Data;
using CANBOOKRAM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CANBOOKRAM.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var requests = _context.FriendRequests.Include("User").Where(i => i.Friend == applicationUser).ToList();
            return View(requests);
        }

        public async Task<IActionResult> RequestList()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var requests = _context.FriendRequests.Include("User").Where(i => i.Friend == applicationUser).ToList();

            return PartialView("_RequestsPartial", requests);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var request = _context.FriendRequests.Include("User").Where(i => i.Id == id && i.Friend == applicationUser).FirstOrDefault();

            if (request != null)
            {
                var follower = _context.UserFriends.Include("Friend").Where(i => i.Friend == applicationUser && i.User == request.User).FirstOrDefault();

                if (follower == null)
                {
                    var model = new UserFriend();
                    model.Friend = applicationUser;
                    model.User = request.User;

                    _context.UserFriends.Add(model);
                    _context.SaveChanges();
                }

                _context.FriendRequests.Remove(request);
                _context.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Request not found" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRequest(int id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var request = _context.FriendRequests.Where(i => i.Id == id && i.Friend == applicationUser).FirstOrDefault();

            if (request != null)
            {
                _context.FriendRequests.Remove(request);
                _context.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Request not found" });
            }
        }
    }
}
