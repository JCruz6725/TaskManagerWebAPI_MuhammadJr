using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

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
        public async Task<ActionResult<TaskDto>> GetTaskById([FromHeader] Guid userId, Guid taskId)
        {
            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
            if (taskItem is null)
            {
                return NotFound(taskId);
            }

            TaskDto? taskDetail = new()                                   //create a new instance of TaskDto and set their properties 
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                DueDate = taskItem.DueDate,
                Priority = taskItem.Priority,
                ParentTaskId = taskItem.SubTaskSubTaskItems.FirstOrDefault()?.TaskItemId,


                CreatedDate = taskItem.CreatedDate,
                CreatedUserId = taskItem.CreatedUserId,

                Notes = taskItem.TaskItemNotes.Select                            //within the TaskDto create a new List of Notes that grabs TaskItemNotes and set their properties
                    (note => new NoteDto                                         //create new instance of NoteDto
                    {
                        Id = note.Id,
                        TaskItemId = note.TaskItemId,
                        Note = note.Note,
                        CreatedDate = note.CreatedDate,
                        CreatedUser = note.CreatedUserId,
                    }).ToList(),                                                 //add notes to the list

                CurrentStatus = taskItem.TaskItemStatusHistories.OrderByDescending(rank => rank.CreatedDate)   //within the TaskDto create a new list of CurrentStatus that grabs task histories and set their properites
                 .Select(history => new StatusDto                                     //create new instance of StatusDto
                 {
                     Id = history.Status.Id,
                     Name = history.Status.Name,
                     Code = history.Status.Code,
                 }).FirstOrDefault(),
            };
            return Ok(taskDetail);                                            //retun task details
        }


        [HttpPost(Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromHeader] Guid userId, TaskCreateDto taskCreatedDto)
        {
            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            if (taskCreatedDto.ParentTaskId.HasValue)
            {
                TaskItem? parentTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskCreatedDto.ParentTaskId.Value, userId);
                if (parentTask is null)
                {
                    return NotFound(taskCreatedDto.ParentTaskId);
                }
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
                CreatedUserId = userId,
                TaskItemStatusHistories = [
                    new TaskItemStatusHistory() {
                        StatusId = _statusChange.PendingId,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = userId
                    }
                ]

            };

            if (taskCreatedDto.DueDate == null)
            {
                taskCreation.DueDate = new DateTime(1900, 1, 1);   //Default if null
            }
            {
                taskCreation.DueDate = taskCreatedDto.DueDate!.Value; //enetered value
            }

            //SubTask creation if ParentId is provided
            if (taskCreatedDto.ParentTaskId.HasValue)
            {
                SubTask subTask = new()
                {
                    TaskItemId = taskCreatedDto.ParentTaskId.Value,
                    SubTaskItemId = taskCreation.Id,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = userId
                };
                taskCreation.SubTaskSubTaskItems.Add(subTask);
            }

            await _unitOfWork.TaskItem.CreateTaskAsync(taskCreation);              //UofW takes the TaskItem class and calls the CreateTask method from the TaskItemRepo
            await _unitOfWork.SaveChangesAsync();                                  //UofW calls the SaveChanges method
            taskCreation = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskCreation.Id, userId) ?? throw new Exception("The object you tried to create does not exist in db");

            //Response DTO
            //create a new instance of TaskDto
            //calls the TaskDto prop and call the taskCreation and set the prop for user view
            //return the result of the tasks created
            TaskDto creationResult = new()
            {
                Id = taskCreation.Id,
                Title = taskCreation.Title,
                DueDate = taskCreation.DueDate,
                Priority = taskCreation.Priority,
                ParentTaskId = taskCreation.SubTaskSubTaskItems.FirstOrDefault()?.TaskItemId,

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
            return CreatedAtAction(nameof(CreateTask), new { taskId = taskCreation.Id }, creationResult);
        }


        [HttpPost("{taskId}/notes", Name = "CreateNote")]
        public async Task<ActionResult<NoteCreateDto>> CreateNote([FromHeader] Guid userId, Guid taskId, NoteCreateDto noteCreateDto)
        {

            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
            if (taskItem is null)
            {
                return NotFound(taskId);
            }

            //Request DTO
            TaskItemNote noteCreation = new TaskItemNote
            {
                TaskItemId = taskId,
                Note = noteCreateDto.NoteText,
                CreatedDate = DateTime.Now,
                CreatedUserId = userId
            };

            await _unitOfWork.TaskItem.CreateNoteAsync(noteCreation);
            await _unitOfWork.SaveChangesAsync();

            //Response DTO
            var noteResult = new NoteDto
            {
                Id = noteCreation.Id,
                TaskItemId = noteCreation.TaskItemId,
                Note = noteCreation.Note,
                CreatedDate = noteCreation.CreatedDate,
                CreatedUser = noteCreation.CreatedUserId
            };

            return CreatedAtAction(nameof(CreateNote), new { id = noteCreation.Id }, noteResult);
        }


        [HttpGet("{taskId}/notes", Name = "GetAllNotes")]
        public Task<ActionResult<List<NoteDto>>> GetAllNotes([FromHeader] Guid userId, Guid taskId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{taskId}/notes/{noteId}", Name = "DeleteNote")]
        public async Task<ActionResult<NoteDto>> DeleteNote([FromHeader] Guid userId, Guid taskId, Guid noteId)
        {

            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
            if (taskItem is null)
            {
                return NotFound(taskId);
            }

            TaskItemNote? note = taskItem.TaskItemNotes.SingleOrDefault(n => n.Id == noteId);
            if (note is null)
            {
                return NotFound(noteId);
            }

            _unitOfWork.TaskItem.DeleteNote(note);
            await _unitOfWork.SaveChangesAsync();

            //Response DTO
            NoteDto deleteNote = new NoteDto
            {
                Id = note.Id,
                TaskItemId = taskId,
                Note = note.Note,
                CreatedDate = note.CreatedDate,
                CreatedUser = note.CreatedUserId,
            };
            return Ok(deleteNote);
        }

        private bool HasIncompletedDescendants(TaskItem task)
        {
            //If task has no subtasks end the check and return false
            if (task.SubTaskTaskItems == null)
            {
                return false;
            }

            //Loop through every single subtask
            foreach (var sub in task.SubTaskTaskItems)
            {
                //Get the child task from the subtask
                //TaskItem child = sub.TaskItem;
                TaskItem child = sub.SubTaskItem;

                //Get the latest status history of a subtask and sort by created date
                TaskItemStatusHistory? latestStatus = child.TaskItemStatusHistories
                                        .OrderByDescending(c => c.CreatedDate)
                                        .FirstOrDefault();
                //Check if the latest status is not complete (StatusId != CompleteId)
                bool inCompletedChild =latestStatus!.StatusId != _statusChange.CompleteId;

                //If task is incomplete return true
                if (inCompletedChild)
                {
                    return true;
                }
                
                //Recursiveley check for any SubTask that is incomplete
                if (HasIncompletedDescendants(child))
                {
                    return true;
                }
            }
            //If all subtasks in descedndants are complete end the check
            return false;
        }

        [HttpPost("{taskId}/status-change/complete", Name = "StatusChangeComplete")]
        public async Task<ActionResult<TaskDto>> StatusChangeComplete([FromHeader] Guid userId, Guid taskId)
        {
            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
            if (taskItem is null)
            {
                return NotFound(taskId);
            }

            // Prevent completing a task when any child SubTask is not complete.
            if (HasIncompletedDescendants(taskItem))
            {
                return BadRequest("Cannot complete parent task with incomplete child sub-tasks.");
            }

            //add new status history for Complete
            //Reuest DTO
            TaskItemStatusHistory newTaskStatus = new()
            {
                TaskItemId = taskItem.Id,
                StatusId = _statusChange.CompleteId,
                CreatedDate = DateTime.Now,
                CreatedUserId = userId,
            };

            taskItem.TaskItemStatusHistories.Add(newTaskStatus);
            await _unitOfWork.SaveChangesAsync();

            //Response DTO
            TaskDto statusResult = new()
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                DueDate = taskItem.DueDate,
                Priority = taskItem.Priority,
                ParentTaskId = taskItem.SubTaskSubTaskItems.FirstOrDefault()?.TaskItemId,

                Notes = taskItem.TaskItemNotes.Select(n => new NoteDto
                {
                    Id = n.Id,
                    TaskItemId = n.TaskItemId,
                    Note = n.Note,
                    CreatedDate = n.CreatedDate,
                    CreatedUser = n.CreatedUserId
                }).ToList(),

                CurrentStatus = new StatusDto
                {
                    Id = _statusChange.CompleteId,
                    Name = _statusChange.Complete,
                    Code = _statusChange.Code2
                },

                CreatedDate = taskItem.CreatedDate,
                CreatedUserId = taskItem.CreatedUserId,
            };
            return CreatedAtAction(nameof(StatusChangeComplete), new { taskId = newTaskStatus.Id }, statusResult);
        }


        [HttpPost("{taskId}/status-change/pending", Name = "StatusChangePending")]
        public async Task<ActionResult<TaskDto>> StatusChangePending([FromHeader] Guid userId, Guid taskId)
        {
            throw new NotImplementedException();
        }


        [HttpPut("{taskId}", Name = "EditTask")]
        public async Task<ActionResult<TaskDto>> EditTask([FromHeader] Guid userId, Guid taskId, TaskDto updateTaskDto)
        {
            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
            if (taskItem is null)
            {
                return NotFound(taskId);
            }

            //check if parent task exists
            if (updateTaskDto.ParentTaskId.HasValue)
            {
                TaskItem? parentTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(updateTaskDto.ParentTaskId.Value, userId);
                if (parentTask is null)
                {
                    return NotFound(updateTaskDto.ParentTaskId);
                }
            }

            //subtask creation if ParentId is provided
            if (updateTaskDto.ParentTaskId.HasValue)
            {
                SubTask? subTask = new()
                {
                    TaskItemId = updateTaskDto.ParentTaskId.Value,
                    SubTaskItemId = taskItem.Id,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = userId
                };
                taskItem.SubTaskSubTaskItems.Add(subTask);
            }
            await _unitOfWork.SaveChangesAsync();

            //update only provided property fields
            if (updateTaskDto.Title != null &&
                updateTaskDto.DueDate.HasValue &&
                updateTaskDto.Priority != 0)
            {
                taskItem.Title = updateTaskDto.Title;
                taskItem.DueDate = updateTaskDto.DueDate.Value;
                taskItem.Priority = updateTaskDto.Priority;
            }
            await _unitOfWork.SaveChangesAsync();

            //Response DTO
            TaskDto editTaskResult = new TaskDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                DueDate = taskItem.DueDate,
                Priority = taskItem.Priority,
                ParentTaskId = taskItem.SubTaskSubTaskItems.FirstOrDefault()?.TaskItemId,

                Notes = taskItem.TaskItemNotes.Select(n => new NoteDto
                {
                    Id = n.Id,
                    TaskItemId = n.TaskItemId,
                    Note = n.Note,
                    CreatedDate = n.CreatedDate,
                    CreatedUser = n.CreatedUserId
                }).ToList(),

                CurrentStatus = taskItem.TaskItemStatusHistories.OrderByDescending(rank => rank.CreatedDate)
                .Select(history => new StatusDto
                {
                    Id = history.Status.Id,
                    Name = history.Status.Name,
                    Code = history.Status.Code,
                }).FirstOrDefault(),

                CreatedDate = taskItem.CreatedDate,
                CreatedUserId = taskItem.CreatedUserId
            };
            return Ok(editTaskResult);
        }

    }
}
