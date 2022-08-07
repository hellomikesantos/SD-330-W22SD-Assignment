namespace SD_330_W22SD_Assignment.Models
{
    public class Reputation
    {
        public int Id { get; set; }
        public int Score { get; set; } = 0;
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
