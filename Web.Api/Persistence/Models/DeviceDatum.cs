using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class DeviceDatum
{
    public Guid Id { get; set; }

    public string IpAdress { get; set; } = null!;

    public string BrowserType { get; set; } = null!;

    public int AccessTime { get; set; }

    public int AccessCount { get; set; }

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;
}
