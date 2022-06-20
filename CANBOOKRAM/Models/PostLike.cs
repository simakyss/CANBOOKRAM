namespace CANBOOKRAM.Models
{
    public class PostLike
    {
        public long Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual UserPost Post { get; set; }
        public DateTime DateTime { get; set; }
    }
}
