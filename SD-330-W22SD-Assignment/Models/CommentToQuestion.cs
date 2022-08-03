namespace SD_330_W22SD_Assignment.Models
{
    public class CommentToQuestion
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public int QuestionId { get; set; }
        //public int UserIdInt{ get; set; }
        public string? UserId { get; set; }
        public Question Question { get; set; }
        public ApplicationUser User { get; set; }
    }
}
