using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class LicenseType
{
    public Guid Id { get; set; }

    public string LicenseTitle { get; set; } = null!;

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
