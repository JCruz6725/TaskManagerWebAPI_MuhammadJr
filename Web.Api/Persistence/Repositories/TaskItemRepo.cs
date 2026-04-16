
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLibrary;
using System.Threading.Tasks;
using Web.Api.scaffolding_temp_folder;

namespace Web.Api.Persistence.Repositories
{
    public class TaskItemRepo
    {
        private readonly TaskManagerAppDBContext _context;
        public TaskItemRepo(TaskManagerAppDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get Task by Id that only pertains to specific user. If not found, returns null.
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.TaskItems.SingleOrDefaultAsync(ti => ti.Id == taskId && ti.CreatedUserId == userId);
        }

        public async Task<TaskItem?> GetTaskParentByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.TaskItems.Include(task => task.SubTaskSubTaskItems)
                                            .SingleOrDefaultAsync(ti => ti.Id == taskId && ti.CreatedUserId == userId);
        }

        public async Task<TaskItem?> GetTaskChildrenByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.TaskItems.Include(task => task.SubTaskTaskItems)
                                            .SingleOrDefaultAsync(ti => ti.Id == taskId && ti.CreatedUserId == userId);
        }

        public async Task<TaskItem?> GetTaskNotesByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.TaskItems.Include(task => task.TaskItemNotes)
                                            .SingleOrDefaultAsync(ti => ti.Id == taskId && ti.CreatedUserId == userId);
        }

        public async Task<TaskItem?> GetTaskTaskWithinListsByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.TaskItems.Include(task => task.TaskWithinLists)
                                            .SingleOrDefaultAsync(ti => ti.Id == taskId && ti.CreatedUserId == userId);
        }

        public async Task<TaskItem?> GetTaskStatusHistoryByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.TaskItems.Include(task => task.TaskItemStatusHistories)
                                                .ThenInclude(stat => stat.Status)
                                            .SingleOrDefaultAsync(ti => ti.Id == taskId && ti.CreatedUserId == userId);
        }

        //public async Task<TaskItem?> GetTaskNotesAndStatusByIdAsync(Guid taskId, Guid userId)
        //{
        //    return await _context.TaskItems.Include(task => task.TaskItemNotes)
        //                                   .Include(task => task.TaskItemStatusHistories)
        //                                        .ThenInclude(stat => stat.Status)
        //                                   .SingleOrDefaultAsync(ti => ti.Id == taskId && ti.CreatedUserId == userId);
        //}

        public async Task CreateTaskAsync(TaskItem taskItem)
        {
            await _context.AddAsync(taskItem);

        }

        public async Task CreateNoteAsync(TaskItemNote taskItemItemNote)
        {
            await _context.AddAsync(taskItemItemNote);
        }

        public IEnumerable<TaskItemNote> GetAllNotes(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public void DeleteNote(TaskItemNote taskItemNote)
        {
            _context.Remove(taskItemNote);
        }
        public async Task DeleteTask(TaskItem taskItem)
        {
            // searches for any task or subtask containing the same Taskitem.ID
            SubTask[] AllSubTask = await _context.SubTasks.Where(st => st.TaskItemId == taskItem.Id || st.SubTaskItemId == taskItem.Id).ToArrayAsync();
            _context.RemoveRange(AllSubTask);

            TaskWithinList[] taskWithinList = await _context.TaskWithinLists.Where(twl => twl.TaskItemId == taskItem.Id).ToArrayAsync();
            _context.RemoveRange(taskWithinList);


            TaskItem taskselection = _context.TaskItems.Single(t => t.Id == taskItem.Id);

            foreach (TaskItemStatusHistory item in taskselection.TaskItemStatusHistories)
            {
                _context.Remove(item);
            }

            foreach (TaskItemNote item in taskselection.TaskItemNotes)
            {
                _context.Remove(item);
            }

            _context.Remove(taskselection);
        }

        public async Task DeleteSubTask(SubTask subTaskItem)
        {
            _context.Remove(subTaskItem);
        }

        public void DeleteTaskWithinLists(TaskWithinList taskWithinLists)
        {
            _context.Remove(taskWithinLists);
        }
    }
}

