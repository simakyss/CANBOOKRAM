namespace CANBOOKRAM.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? Name { get; set; }
        public double AvgRating { get; set; }
        public string? City { get; set; }
        public Gender? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int FollowerCount { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsFollowing { get; set; }
        public int? RelationId { get; set; }
        public bool IsRated { get; set; }
        public int YourRate { get; set; }
        public List<UserPost>? UserPosts { get; set; }
        public List<UserPost>? TopPosts { get; set; }
    }
}
