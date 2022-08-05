namespace SD_330_W22SD_Assignment.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool UpVote { get; set; }
        public bool DownVote { get; set; }
    }
}
