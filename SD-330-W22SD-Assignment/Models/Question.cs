namespace SD_330_W22SD_Assignment.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public ApplicationUser User { get; set; }
        public string? UserId { get; set; }
        public bool IsBeingAnswered { get; set; }
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
        public Question()   
        {
            CreatedDate = DateTime.Today;
            IsBeingAnswered = false;
        }
    }
}
