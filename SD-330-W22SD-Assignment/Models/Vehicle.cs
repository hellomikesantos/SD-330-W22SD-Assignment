namespace SD_330_W22SD_Assignment.Models
{
    public class Vehicle
    {
        public int  Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public ApplicationUser User { get; set; }
        public string? UserId { get; set; }
    }
}
