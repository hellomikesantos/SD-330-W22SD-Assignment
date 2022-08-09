using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Data;

namespace SD_330_W22SD_Assignment.Models
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            ApplicationDbContext context = new ApplicationDbContext
                (serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ADD ROLES

            List<string> roles = new List<string>
            {
                "User", "Administrator", "Moderator"
            };

            if (!context.Roles.Any())
            {
                foreach (string role in roles)
                {

                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                await context.SaveChangesAsync();



                if (!context.Users.Any())
                {
                    ApplicationUser seededUser = new ApplicationUser
                    {
                        Email = "Admin@test.ca",
                        NormalizedEmail = "ADMIN@TEST.CA",
                        UserName = "Admin@test.ca",
                        NormalizedUserName = "ADMIN@TEST.CA",
                        EmailConfirmed = true,
                    };

                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(seededUser, "P@ssword1");
                    seededUser.PasswordHash = hashed;

                    await userManager.CreateAsync(seededUser);
                    await userManager.AddToRoleAsync(seededUser, "Administrator");

                }

                await context.SaveChangesAsync();
            }
        
        }
    }
}
