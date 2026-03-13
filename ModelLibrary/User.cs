using System;
using System.Collections.Generic;

namespace ModelLibrary;

public partial class User
{
    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<DeviceDatum> DeviceData { get; set; } = new List<DeviceDatum>();

    public virtual ICollection<List> Lists { get; set; } = new List<List>();

    public virtual ICollection<Password> Passwords { get; set; } = new List<Password>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    public virtual ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();

    public virtual ICollection<TaskItemNote> TaskItemNotes { get; set; } = new List<TaskItemNote>();

    public virtual ICollection<TaskItemStatusHistory> TaskItemStatusHistories { get; set; } = new List<TaskItemStatusHistory>();

    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    public virtual ICollection<TaskWithinList> TaskWithinLists { get; set; } = new List<TaskWithinList>();
}
