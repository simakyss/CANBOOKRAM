namespace CANBOOKRAM.Models
{
    public class UserFriend
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Friend { get; set; }
    }
}
