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
            var getTasks = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId); //UofW that takes the TaskItem and call the TaskItemRepo GetUserById
            if (getTasks == null)                                               //check if the taskId  is null then return invalid
            {
                return NotFound("Id is invalid");
            }

            var taskDetail = new TaskDto()                                    //create a new instance of TaskDto and set their properties 
            {
                Id = getTasks.Id,
                Title = getTasks.Title,
                DueDate = getTasks.DueDate,
                Priority = getTasks.Priority,
                CreatedDate = getTasks.CreatedDate,
                CreatedUserId = getTasks.CreatedUserId,
                Notes = getTasks.TaskItemNotes.Select                            //within the TaskDto create a new List of Notes that grabs TaskItemNotes and set their properties
                    (note => new NoteDto                                         //create new instance of NoteDto
                    {
                        Id = note.Id,
                        TaskItemId = note.TaskItemId,
                        Note = note.Note,
                        CreatedDate = note.CreatedDate,
                        CreatedUser = note.CreatedUserId,
                    }).ToList(),                                                 //add notes to the list

                CurrentStatus = getTasks.TaskItemStatusHistories.Select           //within the TaskDto create a new list of CurrentStatus that grabs task histories and set their properites
                     (history => new StatusDto                                     //create new instance of StatusDto
                     {
                         Id = history.Status.Id,
                         Name = history.Status.Name,
                         Code = history.Status.Code,
                     }).FirstOrDefault(),
            };
            return Ok(taskDetail);                                            //retun task details

        }

        [HttpPost(Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromHeader] Guid UserId, [FromBody] TaskCreateDto taskCreatedDto)
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
    }
}
