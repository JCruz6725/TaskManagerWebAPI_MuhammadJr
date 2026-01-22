using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? DueDate { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedDate { get; set; }

    public Guid CreatedUserId { get; set; }

    public virtual User CreatedUser { get; set; } = null!;

    public virtual ICollection<SubTask> SubTaskSubTaskItems { get; set; } = new List<SubTask>();

    public virtual ICollection<SubTask> SubTaskTaskItems { get; set; } = new List<SubTask>();

    public virtual ICollection<TaskItemNote> TaskItemNotes { get; set; } = new List<TaskItemNote>();

    public virtual ICollection<TaskItemStatusHistory> TaskItemStatusHistories { get; set; } = new List<TaskItemStatusHistory>();

    public virtual ICollection<TaskWithinList> TaskWithinLists { get; set; } = new List<TaskWithinList>();
}
