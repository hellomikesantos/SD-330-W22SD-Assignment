using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Models;

namespace SD_330_W22SD_Assignment.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {  
        }
        public DbSet<SD_330_W22SD_Assignment.Models.Question>? Question { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Answer>? Answer { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Tag>? Tag { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.CommentToQuestion>? CommentToQuestion{ get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.CommentToAnswer>? CommentToAnswer{ get; set; }
    }
}