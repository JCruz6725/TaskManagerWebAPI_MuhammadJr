using System;
using System.Collections.Generic;

namespace Web.Api;

public partial class User
{
    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int Zipcode { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string Ipaddress { get; set; } = null!;

    public virtual ICollection<List> Lists { get; set; } = new List<List>();

    public virtual ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();

    public virtual ICollection<TaskItemNote> TaskItemNotes { get; set; } = new List<TaskItemNote>();

    public virtual ICollection<TaskItemStatusHistory> TaskItemStatusHistories { get; set; } = new List<TaskItemStatusHistory>();

    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    public virtual ICollection<TaskWithinList> TaskWithinLists { get; set; } = new List<TaskWithinList>();
}
