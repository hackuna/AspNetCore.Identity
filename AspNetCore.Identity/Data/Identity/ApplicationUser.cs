using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Data.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public virtual ICollection<ApplicationUserClaim> Claims { get; set; } = [];
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; } = [];
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; } = [];
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
}
