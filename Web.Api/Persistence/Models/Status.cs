using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class Status
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Code { get; set; }

    public virtual ICollection<TaskItemStatusHistory> TaskItemStatusHistories { get; set; } = new List<TaskItemStatusHistory>();
}
