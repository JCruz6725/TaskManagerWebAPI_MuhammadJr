using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public partial class TaskItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedUserId { get; set; }

        public virtual User CreatedUser { get; set; } = null!;

        public virtual ICollection<SubTask> SubTaskSubTaskItems { get; set; } = new List<SubTask>(); //children tasks

        public virtual ICollection<SubTask> SubTaskTaskItems { get; set; } = new List<SubTask>(); //parent task

        public virtual ICollection<TaskItemNote> TaskItemNotes { get; set; } = new List<TaskItemNote>();

        public virtual ICollection<TaskItemStatusHistory> TaskItemStatusHistories { get; set; } = new List<TaskItemStatusHistory>();

        public virtual ICollection<TaskWithinList> TaskWithinLists { get; set; } = new List<TaskWithinList>();
    }
}
