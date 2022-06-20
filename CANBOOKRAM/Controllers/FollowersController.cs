using CANBOOKRAM.Data;
using CANBOOKRAM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CANBOOKRAM.Controllers
{
    public class FollowersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FollowersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var followers = _context.UserFriends.Include("User").Where(i => i.Friend == applicationUser).ToList();
            return View(followers);
        }

        public async Task<IActionResult> RefreshList()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var followers = _context.UserFriends.Include("User").Where(i => i.Friend == applicationUser).ToList();

            return PartialView("_FollowersPartial", followers);
        }

        [HttpPost]
        public IActionResult RemoveFollower(int id)
        {
            var relation = _context.UserFriends.Where(i => i.Id == id).FirstOrDefault();

            if (relation != null)
            {
                _context.UserFriends.Remove(relation);
                _context.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Relation not found" });
            }
        }
    }
}
