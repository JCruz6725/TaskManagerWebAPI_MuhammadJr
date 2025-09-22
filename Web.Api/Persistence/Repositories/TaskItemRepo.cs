
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Persistence.Models;

namespace Web.Api.Persistence.Repositories
{
    public class TaskItemRepo
    {
        private readonly TaskManagerAppDBContext _context;

        public TaskItemRepo(TaskManagerAppDBContext context)
        { 
            _context = context;  
        }
        public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
        {
            //return await _context.TaskItems.FirstOrDefaultAsync(x => x.Id == id);

            throw new NotImplementedException();
        }

        public async Task CreateTaskAsync(TaskItem taskItem)
        {
            await _context.AddAsync(taskItem);

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

    }
}
