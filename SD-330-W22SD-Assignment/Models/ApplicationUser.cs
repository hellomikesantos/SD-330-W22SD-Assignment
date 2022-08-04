using Microsoft.AspNetCore.Identity;

namespace SD_330_W22SD_Assignment.Models
{
    public class ApplicationUser: IdentityUser
    {
        public ICollection<Vehicle> Vehicles { get; set; } = new HashSet<Vehicle>();
        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
        public ICollection<CommentToAnswer> CommentsToAnswer { get; set; } = new HashSet<CommentToAnswer>();
        public ApplicationUser() : base()
        {

        }
    }
}
