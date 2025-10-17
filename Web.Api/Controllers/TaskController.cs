using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
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
        private readonly StatusChange _statusChange;
        private readonly ILogger<TaskController> _logger;
        public TaskController(UnitOfWork unitOfWork, IOptions<StatusChange> statusChangeOptions, ILogger<TaskController> logger)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
            _statusChange = statusChangeOptions.Value;
            _logger = logger;
        }

        [HttpGet("{taskId}", Name = "GetTaskById")]
        public async Task<ActionResult<TaskDto>> GetTaskById([FromHeader]Guid userId, Guid taskId)
        {
            TaskItem? getTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);    //UofW that takes the TaskItem and call the TaskItemRepo GetUserById
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);
            
            if (getTask == null && getUser == null)                                               
            {
                return NotFound($"UserId {userId} and TaskId {taskId} are invalid");
            }
            if(getTask == null && getUser != null)
            {
                return NotFound($"TaskId {taskId} is invalid");
            } 
            if(getTask != null &&  getUser == null)
            {
                return NotFound($"UserId {userId} is invalid");
            }
            if (getTask.CreatedUserId != getUser.Id)
            {
                return Unauthorized($"Task {taskId } does not belog to this user {userId} ");
            }

            TaskDto? taskDetail = new TaskDto                                   //create a new instance of TaskDto and set their properties 
            {
                Id = getTask.Id,
                Title = getTask.Title,
                DueDate = getTask.DueDate,
                Priority = getTask.Priority,
                CreatedDate = getTask.CreatedDate,
                CreatedUserId = getTask.CreatedUserId,
                Notes = getTask.TaskItemNotes.Select                            //within the TaskDto create a new List of Notes that grabs TaskItemNotes and set their properties
                    (note => new NoteDto                                         //create new instance of NoteDto
                    {
                        Id = note.Id,
                        TaskItemId = note.TaskItemId,
                        Note = note.Note,
                        CreatedDate = note.CreatedDate,
                        CreatedUser = note.CreatedUserId,
                    }).ToList(),                                                 //add notes to the list

                CurrentStatus = getTask.TaskItemStatusHistories.OrderByDescending(rank => rank.CreatedDate)   //within the TaskDto create a new list of CurrentStatus that grabs task histories and set their properites
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
            TaskDto creationResult = new TaskDto()
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
        public async Task<ActionResult<NoteDto>> DeleteNote([FromHeader]Guid userId, Guid taskId, Guid noteId)
        {
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);
            TaskItem? getTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);

            if (getUser == null)
            {
                return NotFound($"UserId {userId} is invalid");
            }
            if (getTask == null)
            {
                return NotFound($"TaskId {taskId} is invalid");
            }
            if (getTask.CreatedUserId != getUser.Id)
            {
                return Unauthorized($"TaskId {taskId} does not belong to this UserId {userId}");
            }

            TaskItemNote? getNote =  getTask.TaskItemNotes.Where(n => n.Id == noteId).FirstOrDefault();
             _unitOfWork.TaskItem.DeleteNote(getNote);
            await _unitOfWork.SaveChangesAsync();

            NoteDto deleteNote = new NoteDto
            {
                Id = getNote.Id,
                TaskItemId = taskId,
                Note = getNote.Note,
                CreatedDate = getNote.CreatedDate,
                CreatedUser = getNote.CreatedUserId,
            };
            return Ok(deleteNote);
        }

        [HttpPost("{taskId}/status-change/complete", Name = "StatusChangeComplete")]
        public async Task<ActionResult<TaskDto>> StatusChangeComplete([FromHeader]Guid userId, Guid taskId)
        {
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);
            TaskItem? getTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);

            if(getUser == null)
            {
                 return NotFound($"UserId {userId} is invalid");
            }
            if(getTask == null)
            {
                return NotFound($"TaskId {taskId} is invalid");
            }
            if(getTask.CreatedUserId != getUser.Id)
            {
                return Unauthorized($"TaskId {taskId} does not belong to this UserId {userId}");
            }

            TaskItemStatusHistory statusHistory = new TaskItemStatusHistory
            {
                TaskItemId = getTask.Id,
                StatusId = _statusChange.CompleteId,
                CreatedDate = DateTime.Now,
                CreatedUserId = getUser.Id
            };

            var taskStatus = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);
            taskStatus.TaskItemStatusHistories.Add(statusHistory);
            await _unitOfWork.SaveChangesAsync();

            TaskDto statusResult = new TaskDto
            {
                Id = getTask.Id,
                Title = getTask.Title,
                DueDate = getTask.DueDate,
                Priority = getTask.Priority,

                Notes = getTask.TaskItemNotes.Select(n => new NoteDto 
                {
                    Id =n.Id,
                    TaskItemId = n.TaskItemId,
                    Note = n.Note,
                    CreatedDate = n.CreatedDate,
                    CreatedUser = n.CreatedUserId
                }).ToList(),
                
                CurrentStatus = new StatusDto
                {
                   Id = getTask.Id,
                   Name = _statusChange.Complete,
                   Code = _statusChange.Code2
                },
               
               CreatedDate = getTask.CreatedDate,
               CreatedUserId = getTask.CreatedUserId,
            };
            return CreatedAtAction(nameof(StatusChangeComplete), new { taskId = statusHistory.Id }, statusResult);
        }

        [HttpPost("{taskId}/status-change/pending", Name = "StatusChangePending")]
        public async Task<ActionResult<TaskDto>> StatusChangePending([FromHeader]Guid userId, Guid taskId)
        {
            throw new NotImplementedException();
        }
    }
}
