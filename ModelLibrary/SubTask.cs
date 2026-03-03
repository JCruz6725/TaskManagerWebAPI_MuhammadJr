using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public partial class SubTask
    {
        public Guid Id { get; set; }

        public Guid TaskItemId { get; set; }

        public Guid SubTaskItemId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedUserId { get; set; }

        public virtual User CreatedUser { get; set; } = null!;

        public virtual TaskItem SubTaskItem { get; set; } = null!;

        public virtual TaskItem TaskItem { get; set; } = null!;
    }
}
