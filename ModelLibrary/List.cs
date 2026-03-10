
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public partial class List
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public Guid CreatedUserId { get; set; }

        public virtual User CreatedUser { get; set; } = null!;

        public virtual ICollection<TaskWithinList> TaskWithinLists { get; set; } = new List<TaskWithinList>();
    }
}
