
using Web.Api.Persistence.Models;

namespace Web.Api.Persistence.Repositories
{
    public class TaskItemRepo
    {
        public TaskItem GetTaskById(Guid Id)
        {
            throw new NotImplementedException();

        }

        public void CreateTask(TaskItem taskItem)
        {
            throw new NotImplementedException();

        }
        public void CreateNote(TaskItemNote taskItemItemNotes)
        {
            throw new NotImplementedException();

        }
        public IEnumerable<TaskItemNote>  GetAllNotes(Guid taskId)
        {
            throw new NotImplementedException();

        }

        public void DeleteNote(Guid TaskItemNoteId)
        {
            throw new NotImplementedException();

        }

        private readonly AppDbContext _context;
    }
}
