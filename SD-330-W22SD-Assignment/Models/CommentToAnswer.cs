namespace SD_330_W22SD_Assignment.Models
{
    public class CommentToAnswer
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public int AnswerId { get; set; }
        public string? UserId { get; set; }
        public Answer Answer { get; set; }
        public ApplicationUser User { get; set; }
    }
}
