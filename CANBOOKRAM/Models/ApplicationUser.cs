using Microsoft.AspNetCore.Identity;

namespace CANBOOKRAM.Models
{
    public enum Gender {
        Male = 0,   
        Female = 1,
    }

    public class ApplicationUser : IdentityUser
    {
        public string? City { get; set; }
        public Gender? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? Name { get; set; }
        public bool IsPrivate { get; set; }

        public virtual ICollection<UserPost> Posts { get; set; }
    }
}
