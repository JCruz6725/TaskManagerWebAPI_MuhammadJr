using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
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
        public async Task<ActionResult<TaskDto>> GetTaskById([FromHeader]Guid userId, Guid taskId)
        {
            TaskItem? getTasks = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);    //UofW that takes the TaskItem and call the TaskItemRepo GetUserById
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);
            
            if (getTasks == null && getUser == null)                                               
            {
                return NotFound($"UserId {userId} and TaskId {taskId} are invalid");
            }
            if(getTasks == null && getUser != null)
            {
                return NotFound($"TaskId {taskId} is invalid");
            } 
            if(getTasks != null &&  getUser == null)
            {
                return NotFound($"UserId {userId} is invalid");
            }
            if (getTasks.CreatedUserId != getUser.Id)
            {
                return Unauthorized($"Task {taskId } does not belog to this user {userId} ");
            }

            TaskDto? taskDetail = new TaskDto                                   //create a new instance of TaskDto and set their properties 
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

                CurrentStatus = getTasks.TaskItemStatusHistories.OrderByDescending(rank => rank.CreatedDate)   //within the TaskDto create a new list of CurrentStatus that grabs task histories and set their properites
                 .Select (history => new StatusDto                                     //create new instance of StatusDto
                     {
                         Id = history.Status.Id,
                         Name = history.Status.Name,
                         Code = history.Status.Code,
                     }).FirstOrDefault(),
            };
            return Ok(taskDetail);                                            //retun task details

        }

        [HttpPost(Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromHeader]Guid userId, TaskCreateDto taskCreatedDto) { 
            //Request DTO
            //create a new instance of TaskItem 
            User? userExist = await _unitOfWork.User.GetUserByIdAsync(userId);  //Check if user exists before adding task
            if (userExist is null)
            {
                return NotFound("user account does not exist");
            }
            //calls the TaskItem prop and set the task created dto to its prop
            //Request DTO
            //create a new instance of TaskItem 
            //calls the TaskItem prop and set the task created dto to its prop
            TaskItem? taskCreation = new TaskItem()
            {
                Title = taskCreatedDto.Title,
                //DueDate = taskCreatedDto.DueDate == default ? taskCreatedDto.DueDate.Value : DateTime.Now.AddDays(1),
                Priority = taskCreatedDto.Priority,
                CreatedDate = DateTime.Now,
                CreatedUserId = userId                                              //set the UserId which is given by the user from the header
            };

            if(taskCreatedDto.DueDate == null)
            {
                taskCreation.DueDate = new DateTime(1900, 1, 1);   //Default if null
            }
            {
                taskCreation.DueDate = taskCreatedDto.DueDate.Value; //enetered value
            }

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

                Notes = taskCreation.TaskItemNotes.Select
                    (note => new NoteDto
                    {
                        Id = note.Id,
                        TaskItemId = note.TaskItemId,
                        Note = note.Note,
                        CreatedDate = note.CreatedDate,
                        CreatedUser = note.CreatedUserId,
                    }).ToList(),

                CurrentStatus = taskCreation.TaskItemStatusHistories.OrderByDescending(rank => rank.CreatedDate)
                .Select(history => new StatusDto                                     
                     {
                         Id = history.Status.Id,
                         Name = history.Status.Name,
                         Code = history.Status.Code,
                     }).FirstOrDefault(),

                CreatedDate = taskCreation.CreatedDate,
                CreatedUserId = taskCreation.CreatedUserId

            };
            return CreatedAtAction(nameof(CreateTask),new {taskId = taskCreation.Id}, creationResult);
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
