using System;
using System.Collections.Generic;

namespace Web.Api.Persistence.Models;

public partial class User
{
    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<List> Lists { get; set; } = new List<List>();

    public virtual ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();

    public virtual ICollection<TaskItemNote> TaskItemNotes { get; set; } = new List<TaskItemNote>();

    public virtual ICollection<TaskItemStatusHistory> TaskItemStatusHistories { get; set; } = new List<TaskItemStatusHistory>();

    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    public virtual ICollection<TaskWithinList> TaskWithinLists { get; set; } = new List<TaskWithinList>();
}
