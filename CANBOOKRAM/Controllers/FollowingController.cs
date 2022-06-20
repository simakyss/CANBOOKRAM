using CANBOOKRAM.Data;
using CANBOOKRAM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CANBOOKRAM.Controllers
{
    public class FollowingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FollowingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var following = _context.UserFriends.Include("Friend").Where(i => i.User == applicationUser).ToList();
            return View(following);
        }

        public async Task<IActionResult> RefreshList()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            var following = _context.UserFriends.Include("Friend").Where(i => i.User == applicationUser).ToList();

            return PartialView("_FollowingPartial", following);
        }

        [HttpPost]
        public IActionResult RemoveFollowing(int id)
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
