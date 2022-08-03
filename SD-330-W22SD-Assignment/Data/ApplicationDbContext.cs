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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Question_Tag>()
        //            .HasOne(q => q.Question)
        //            .WithMany(qa => qa.QuestionTags)
        //            .HasForeignKey(qa => qa.QuestionId);

        //    modelBuilder.Entity<Question_Tag>()
        //            .HasOne(t => t.Tag)
        //            .WithMany(qt => qt.QuestionTags)
        //            .HasForeignKey(qt => qt.TagId);
        //}
        public DbSet<SD_330_W22SD_Assignment.Models.Vehicle>? Vehicle { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Question>? Question { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Answer>? Answer { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Tag>? Tag { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.CommentToQuestion>? CommentToQuestion{ get; set; }


    }
}