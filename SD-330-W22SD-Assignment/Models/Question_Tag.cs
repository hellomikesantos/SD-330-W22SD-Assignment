namespace SD_330_W22SD_Assignment.Models
{
    public class Question_Tag
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }

    }
}
