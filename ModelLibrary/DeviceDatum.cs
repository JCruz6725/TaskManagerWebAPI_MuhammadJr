using System;
using System.Collections.Generic;

namespace ModelLibrary;//Web.Api.scaffolding_temp_folder;

public partial class DeviceDatum
{
    public Guid Id { get; set; }

    public string IpAddress { get; set; } = null!;

    public string BrowserType { get; set; } = null!;

    public DateTime AccessTime { get; set; }

        public int AccessCount { get; set; }

        public int AccessCount { get; set; }

        public int AccessCount { get; set; }

        public int AccessCount { get; set; }

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;
}
