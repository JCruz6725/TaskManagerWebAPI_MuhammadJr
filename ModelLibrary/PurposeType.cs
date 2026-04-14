using System;
using System.Collections.Generic;

namespace ModelLibrary;

public partial class PurposeType
{
    public Guid Id { get; set; }

    public string PurposeTitle { get; set; } = null!;

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
