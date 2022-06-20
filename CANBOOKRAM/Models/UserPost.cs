using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CANBOOKRAM.Models
{
    public class UserPost
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public virtual ApplicationUser User { get; set; }
        public byte[]? Image { get; set; }
        public virtual ICollection<PostLike> Likes { get; set; }
    }

}
