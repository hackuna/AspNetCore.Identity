﻿using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Data.Identity;

public class ApplicationUserLogin : IdentityUserLogin<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
