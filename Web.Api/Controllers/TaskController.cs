using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public TaskController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("{taskId}", Name = "GetTaskById")]
        public async Task<ActionResult<TaskDto>> GetTaskById([FromHeader]Guid userId, Guid taskId)
        {
            throw new NotImplementedException();
        }

        [HttpPost( Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromHeader]Guid userId, TaskCreateDto taskCreatedDto)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{taskId}/notes", Name = "CreateNote")]
        public async Task<ActionResult<NoteCreateDto>> CreateNote([FromHeader]Guid userId, Guid taskId, NoteCreateDto noteCreateDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{taskId}/notes", Name = "GetAllNotes")]
        public Task<ActionResult<List<NoteDto>>> GetAllNotes([FromHeader]Guid userId, Guid taskId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{taskId}/notes/{noteId}", Name = "DeleteNote")]
        public async Task<ActionResult<NoteDto>> DeleteNote([FromHeader]Guid userId, Guid NoteId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{taskId}/status-change/complete", Name = "StatusChangeComplete")]
        public async Task<ActionResult<TaskDto>> StatusChangeComplete([FromHeader]Guid userId, Guid TaskId)
        {
            throw new NotImplementedException();

        }

        [HttpPost("{taskId}/status-change/pending", Name = "StatusChangePending")]
        public async Task<ActionResult<TaskDto>> StatusChangePending([FromHeader]Guid userId, Guid TaskId)
        {
            throw new NotImplementedException();
        }
    }
}
