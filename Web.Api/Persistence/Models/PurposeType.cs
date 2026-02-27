using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class PurposeType
{
    public Guid Id { get; set; }

    public string PurposeTitle { get; set; } = null!;

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
