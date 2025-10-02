using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class TaskItemStatusHistory
{
    public Guid Id { get; set; }

    public Guid TaskItemId { get; set; }

    public Guid StatusId { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual TaskItem TaskItem { get; set; } = null!;
}
