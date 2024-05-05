using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Data.Identity;

public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
