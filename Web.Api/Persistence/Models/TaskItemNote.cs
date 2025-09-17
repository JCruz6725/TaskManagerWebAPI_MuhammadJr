using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class TaskItemNote
{
    public Guid Id { get; set; }

    public Guid TaskItemId { get; set; }

    public string Note { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;

    public virtual TaskItem TaskItem { get; set; } = null!;
}
