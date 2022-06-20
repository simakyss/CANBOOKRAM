using CANBOOKRAM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CANBOOKRAM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserPost> UserPosts { get; set; } // tablo oluşturmak için
        public DbSet<PostLike> PostLikes { get; set; } // tablo oluşturmak için
        public DbSet<UserFriend> UserFriends { get; set; } // tablo oluşturmak için
        public DbSet<FriendRequest> FriendRequests { get; set; } // tablo oluşturmak için
        public DbSet<UserRating> UserRatings { get; set; } // tablo oluşturmak için
    }
}