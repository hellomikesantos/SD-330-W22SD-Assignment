namespace SD_330_W22SD_Assignment.Models.ViewModels
{
    public class AnswerAndVote
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
        public int VoteId { get; set; }
        public Vote Vote { get; set; }
    }
}
