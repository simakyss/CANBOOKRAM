namespace CANBOOKRAM.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Friend { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsAccept { get; set; }
    }
}
