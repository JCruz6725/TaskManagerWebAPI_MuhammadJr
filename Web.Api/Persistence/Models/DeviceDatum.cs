using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class DeviceDatum
{
    public Guid Id { get; set; }

    public string IpAddress { get; set; } = null!;

    public string BrowserType { get; set; } = null!;

    public DateTime AccessTime { get; set; }

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;
}
