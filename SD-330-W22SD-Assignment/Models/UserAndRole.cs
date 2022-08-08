
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_330_W22SD_Assignment.Models
{
    public class UserAndRole
    {
        public ICollection<SelectListItem> Users { get; set; }
        public ICollection<SelectListItem> Roles { get; set; }
        public string? Message { get; set; }
        public UserAndRole(ICollection<ApplicationUser> users, ICollection<IdentityRole> identityRoles)
        {
            Users = new HashSet<SelectListItem>();
            Roles = new HashSet<SelectListItem>();

            foreach (ApplicationUser user in users)
            {
                Users.Add(new SelectListItem(user.UserName, user.Id));
            }

            foreach (IdentityRole role in identityRoles)
            {
                Roles.Add(new SelectListItem(role.Name, role.Id));
            }
        }
    }
}
