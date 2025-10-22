
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
        public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId)
        {
            return await _context.TaskItems.Include(item => item.TaskItemNotes).Include(history => history.TaskItemStatusHistories)
                .ThenInclude(stat => stat.Status).FirstOrDefaultAsync(ti => ti.Id == taskId);
        }
        public async Task CreateTaskAsync(TaskItem taskItem)
        {
            await _context.AddAsync(taskItem);

        }
        public async Task CreateNoteAsync(TaskItemNote taskItemItemNote)
        {
            await _context.AddAsync(taskItemItemNote);

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
