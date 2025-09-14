using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using Web.Api.Persistence.Repositories;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : Controller
    {
        private readonly TaskManagerAppDBContext _dbContext;
        public TaskController(TaskManagerAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("{taskId}", Name = "GetTaskById")]
        public TaskDto GetTaskById(Guid taskId)
        {
            throw new NotImplementedException();
        }

        [HttpPost( Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask(TaskCreateDto taskCreatedDto)
        {
            //_dbContext.TaskItems.Add(taskCreatedDto);
            //await _dbContext.SaveChangesAsync();

            ////return CreatedAtAction(nameof())
            //return Ok(taskCreatedDto);

            Console.WriteLine();
            throw new NotImplementedException();
        }

        [HttpPost("{taskId}/notes", Name = "CreateNote")]
        public NoteCreateDto CreateNote(Guid taskId, NoteCreateDto noteCreateDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{taskId}/notes", Name = "GetAllNotes")]
        public List<NoteDto> GetAllNotes(Guid taskId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{taskId}/notes/{noteId}", Name = "DeleteNote")]
        public void DeleteNote(Guid NoteId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{taskId}/status-change/complete", Name = "StatusChangeComplete")]
        public void StatusChangeComplete(Guid TaskId)
        {
            throw new NotImplementedException();

        }

        [HttpPost("{taskId}/status-change/pending", Name = "StatusChangePending")]
        public void StatusChangePending(Guid TaskId)
        {
            throw new NotImplementedException();

        }

        private readonly TaskItemRepo _taskRepsitory;

    }
}
