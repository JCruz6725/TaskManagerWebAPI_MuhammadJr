using Web.Api.Persistence.Repositories;

namespace Web.Api.Persistence
{
    public class UnitOfWork
    {
        public TaskItemRepo TaskItem { get; set; }
        public UserRepo User { get; set; }
        public ListRepo List { get; set; }
    }
}
