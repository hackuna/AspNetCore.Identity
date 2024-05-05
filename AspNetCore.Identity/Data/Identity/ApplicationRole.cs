using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Data.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = [];
}
