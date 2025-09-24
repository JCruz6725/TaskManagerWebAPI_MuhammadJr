using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;
using Web.Api.Persistence.Repositories;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;                         //private readonly field to access the UofW class
        public TaskController(UnitOfWork unitOfWork)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("{taskId}", Name = "GetTaskById")]
        public async Task<ActionResult<TaskDto>> GetTaskById(Guid taskId)
        {
            var getTasks = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);
            if(getTasks == null)
            {
                return NotFound("Id is invalid");
            }

            var taskDetail = new TaskDto()
            {
                Id = getTasks.Id,
                Title = getTasks.Title,
                DueDate = getTasks.DueDate,
                Priority = getTasks.Priority,
                CreatedDate = getTasks.CreatedDate,
                CreatedUserId = getTasks.CreatedUserId,

                Notes = getTasks.TaskItemNotes.Select(note =>  new NoteDto
                {
                    Id = note.Id,
                    TaskItemId = note.TaskItemId,
                    Note = note.Note,
                    CreatedDate = note.CreatedDate,
                    CreatedUser = note.CreatedUserId,
                } ).ToList(), 

                CurrentStatus = getTasks.TaskItemStatusHistories.Select(history => new StatusDto
                {
                    Id = history.Status.Id,
                    Name = history.Status.Name,  
                    Code = history.Status.Code,
                }).FirstOrDefault(),
            };
            return Ok(taskDetail);

        }

        [HttpPost( Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromHeader] Guid UserId,[FromBody]TaskCreateDto taskCreatedDto)
        {
                                                                                        //Request DTO
                                                                                        //create a new instance of TaskItem 
                                                                                       //calls the TaskItem prop and set the task created dto to its prop
            var taskCreation = new TaskItem()                                            
            {
                Title = taskCreatedDto.Title,
                DueDate = taskCreatedDto.DueDate ?? DateTime.Now.AddDays(5),
                Priority = taskCreatedDto.Priority,
                CreatedDate = DateTime.Now,
                CreatedUserId = UserId                                              //set the UserId which is given by the user from the header
            };

            await _unitOfWork.TaskItem.CreateTaskAsync(taskCreation);              //UofW takes the TaskItem class and calls the CreateTask method from the TaskItemRepo
            await _unitOfWork.SaveChangesAsync();                                  //UofW calls the SaveChanges method

                                                                                   //Response DTO
                                                                                   //create a new instance of TaskDto
                                                                                  //calls the TaskDto prop and call the taskCreation and set the prop for user view
                                                                                  //return the result of the tasks created
            var creationResult = new TaskDto()
            {
                Id = taskCreation.Id,
                Title = taskCreation.Title,
                DueDate = taskCreation.DueDate,
                Priority = taskCreation.Priority,
                CreatedDate = taskCreation.CreatedDate,
                CreatedUserId = taskCreation.CreatedUserId,
            };
            return Ok(creationResult);
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

        //private readonly TaskItemRepo _taskRepsitory;

    }
}
