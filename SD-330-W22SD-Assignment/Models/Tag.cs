namespace SD_330_W22SD_Assignment.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Question> Question { get; set; } = new HashSet<Question>();
    }
}
