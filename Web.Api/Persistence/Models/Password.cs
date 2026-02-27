using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class Password
{
    public Guid Id { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;
}
