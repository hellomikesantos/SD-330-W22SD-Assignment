namespace SD_330_W22SD_Assignment.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public int QuestionId { get; set; }
        //public Question Question { get; set; }
        public ICollection<Question> Questions { get; set; } = new HashSet<Question>();
        //public ICollection<QuestionTag> QuestionTags { get; set; } 
    }
}
