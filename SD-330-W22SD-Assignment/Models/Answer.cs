using SD_330_W22SD_Assignment.Models.ViewModels;

namespace SD_330_W22SD_Assignment.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public string Body { get; set; }
        public ApplicationUser User { get; set; }
        public string? UserId { get; set; }
        public ICollection<CommentToAnswer> Comments { get; set; } = new HashSet<CommentToAnswer>();
        public ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
        public ICollection<AnswerAndVote> AnswerAndVote { get; set; }
        public CorrectAnswer? CorrectAnswer { get; set; }
    }
}