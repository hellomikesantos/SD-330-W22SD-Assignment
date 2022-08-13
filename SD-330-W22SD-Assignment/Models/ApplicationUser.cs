using Microsoft.AspNetCore.Identity;

namespace SD_330_W22SD_Assignment.Models
{
    public class ApplicationUser: IdentityUser
    {
        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
        public ICollection<CommentToAnswer> CommentsToAnswer { get; set; } = new HashSet<CommentToAnswer>();
        public ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
        public Reputation Reputation { get; set; } = new Reputation();
        public ApplicationUser() : base()
        {

        }
    }
}
