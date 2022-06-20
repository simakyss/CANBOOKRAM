namespace CANBOOKRAM.Models
{
    public class HomeModel
    {
        public UserPost UserPost { get; set; }
        public List<UserPost> UserPosts{ get; set; }
        public List<ApplicationUser> Users { get; set; }
        public int PostCount { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public List<UserPost> TrendingPosts { get; set; }
    }

}
