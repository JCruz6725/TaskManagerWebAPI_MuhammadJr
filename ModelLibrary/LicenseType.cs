using System;
using System.Collections.Generic;

namespace ModelLibrary;

public partial class LicenseType
{
    public Guid Id { get; set; }

    public string LicenseTitle { get; set; } = null!;

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
