using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using ModelLibrary;
//using Microsoft.Extensions.Logging;

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
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating GetTaskById method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"User {userId} not authorized");
                        return StatusCode(403);
                    }

                    TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
                    if (taskItem is null)
                    {
                        _logger.LogWarning($"TaskId {taskId} not found for UserId {userId}");
                        return NotFound(taskId);
                    }

                    TaskDto? taskDetail = new TaskDto                                   //create a new instance of TaskDto and set their properties 
                    {
                        Id = taskItem.Id,
                        Title = taskItem.Title,
                        DueDate = taskItem.DueDate,
                        Priority = taskItem.Priority,
                        CreatedDate = taskItem.CreatedDate,
                        CreatedUserId = taskItem.CreatedUserId,
                        ParentTaskId = taskItem.SubTaskSubTaskItems.SingleOrDefault()?.TaskItemId,
                        Notes = taskItem.TaskItemNotes.Select                            //within the TaskDto create a new List of Notes that grabs TaskItemNotes and set their properties
                            (note => new NoteDto                                         //create new instance of NoteDto
                            {
                                Id = note.Id,
                                TaskItemId = note.TaskItemId,
                                Note = note.Note,
                                CreatedDate = note.CreatedDate,
                                CreatedUser = note.CreatedUserId,
                            }).OrderByDescending(x => x.CreatedDate).ToList(),                                                 //add notes to the list

                        CurrentStatus = taskItem.TaskItemStatusHistories.OrderByDescending(rank => rank.CreatedDate)   //within the TaskDto create a new list of CurrentStatus that grabs task histories and set their properites
                         .Select(history => new StatusDto                                     //create new instance of StatusDto
                         {
                             Id = history.Status.Id,
                             Name = history.Status.Name,
                             Code = history.Status.Code,
                         }).First(),
                    };
                    _logger.LogInformation($"GetTaskById method successful for TaskId {taskId} and UserId {userId}");
                    _logger.LogInformation("Returning get task by Id result");
                    return Ok(taskDetail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get task by id process failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost(Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromHeader] Guid userId, TaskCreateDto taskCreatedDto)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating CreateTask method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
                        return StatusCode(403);
                    }

                    if (taskCreatedDto.ParentTaskId.HasValue)
                    {
                        TaskItem? parentTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskCreatedDto.ParentTaskId.Value, userId);
                        if (parentTask is null)
                        {
                            _logger.LogWarning($"Parent task {taskCreatedDto.ParentTaskId} not authorized");
                            return NotFound(taskCreatedDto.ParentTaskId);
                        }
                    }

                    if (taskCreatedDto.ListId.HasValue)
                    {
                        List? list = await _unitOfWork.List.GetListByIdAsync(taskCreatedDto.ListId.Value, userId);
                        if (list is null)
                        {
                            _logger.LogWarning($"List {taskCreatedDto.ListId} not authorized");
                            return NotFound(taskCreatedDto.ListId);
                        }
                    }

                    if (taskCreatedDto.Priority < 0)
                    {
                        _logger.LogWarning($"Priority cannot be less than zero. Requested priority is: {taskCreatedDto.Priority}");
                        return BadRequest(taskCreatedDto.Priority);
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
                    else
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

                    //Add task to list if listId provided
                    if (taskCreatedDto.ListId.HasValue)
                    {
                        List? list = await _unitOfWork.List.GetListByIdAsync(taskCreatedDto.ListId.Value, userId);
                        if (list != null)
                        {
                            list.TaskWithinLists.Add(
                                new TaskWithinList()
                                {
                                    CreatedDate = DateTime.Now,
                                    CreatedUserId = userId,
                                    TaskItem = taskCreation,
                                    TaskListId = list.Id
                                }
                            );
                        }
                    }

                    await _unitOfWork.TaskItem.CreateTaskAsync(taskCreation);              //UofW takes the TaskItem class and calls the CreateTask method from the TaskItemRepo
                    await _unitOfWork.SaveChangesAsync();                                  //UofW calls the SaveChanges method
                    taskCreation = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskCreation.Id, userId);

                    _logger.LogInformation($"Task Creation is Successfull for userId {userId}");

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
                        }).First(),

                        CreatedDate = taskCreation.CreatedDate,
                        CreatedUserId = taskCreation.CreatedUserId
                    };
                    _logger.LogInformation($"Created task result for TaskId {taskCreation.Id} and UserId {userId}");
                    _logger.LogInformation("Returning the created task result");
                    return CreatedAtAction(nameof(CreateTask), new { taskId = taskCreation.Id }, creationResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create task process failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("{taskId}/notes", Name = "CreateNote")]
        public async Task<ActionResult<NoteCreateDto>> CreateNote([FromHeader] Guid userId, Guid taskId, NoteCreateDto noteCreateDto)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating CreateNote method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
                        return StatusCode(403);
                    }

                    TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
                    if (taskItem is null)
                    {
                        _logger.LogWarning($"TaskId {taskId} not found for UserId {userId}");
                        return NotFound(taskId);
                    }

                    TaskItemNote noteCreation = new TaskItemNote
                    {
                        TaskItemId = taskId,
                        Note = noteCreateDto.NoteText,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = userId
                    };

                    await _unitOfWork.TaskItem.CreateNoteAsync(noteCreation);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Note Creation is Successfull for userId {userId}");

                    //Response DTO
                    var noteResult = new NoteDto
                    {
                        Id = noteCreation.Id,
                        TaskItemId = noteCreation.TaskItemId,
                        Note = noteCreation.Note,
                        CreatedDate = noteCreation.CreatedDate,
                        CreatedUser = noteCreation.CreatedUserId
                    };
                    _logger.LogInformation($"Created note result for NoteId {noteCreation.Id} and UserId {userId}");
                    _logger.LogInformation("Returning the created note result");
                    return CreatedAtAction(nameof(CreateNote), new { id = noteCreation.Id }, noteResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create note process failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("{taskId}/notes", Name = "GetAllNotes")]
        public Task<ActionResult<List<NoteDto>>> GetAllNotes([FromHeader] Guid userId, Guid taskId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{taskId}/notes/{noteId}", Name = "DeleteNote")]
        public async Task<ActionResult<NoteDto>> DeleteNote([FromHeader] Guid userId, Guid taskId, Guid noteId)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating DeleteNote method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
                        return StatusCode(403);
                    }

                    TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
                    if (taskItem is null)
                    {
                        _logger.LogWarning($"TaskId {taskId} not found for UserId {userId}");
                        return NotFound(taskId);
                    }

                    TaskItemNote? note = taskItem.TaskItemNotes.SingleOrDefault(n => n.Id == noteId);
                    if (note is null)
                    {
                        _logger.LogWarning($"NoteId {noteId} not found for TaskId {taskId} and UserId {userId}");
                        return NotFound(noteId);
                    }

                    _unitOfWork.TaskItem.DeleteNote(note);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Note Deletion is Successfull for userId {userId}");

                    NoteDto deleteNote = new NoteDto
                    {
                        Id = note.Id,
                        TaskItemId = taskId,
                        Note = note.Note,
                        CreatedDate = note.CreatedDate,
                        CreatedUser = note.CreatedUserId,
                    };
                    _logger.LogInformation($"Deleted note result for NoteId {note.Id}, TaskId {taskId} and UserId {userId}");
                    _logger.LogInformation("Returning the deleted note result");
                    return Ok(deleteNote);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete note process failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpDelete("{taskId}", Name = "DeleteTaskById")]
        public async Task<ActionResult<TaskDto>> DeleteTaskById([FromHeader] Guid userId, Guid taskId)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating Delete Task By Id Method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"User id {userId} not authorized");
                        return StatusCode(403);
                    }
                    TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
                    if (taskItem is null)
                    {
                        _logger.LogWarning($"Task item {taskId} not found for user {userId}");
                        return NotFound(taskId);
                    }

                    await _unitOfWork.TaskItem.DeleteTask(taskItem);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Successfully deleted task item {taskId} for user {userId}");

                    TaskDto deleteTask = new TaskDto
                    {
                        Id = taskItem.Id,
                        Title = taskItem.Title,
                        DueDate = taskItem.DueDate,
                        Priority = taskItem.Priority,
                        Notes = taskItem.TaskItemNotes.Select(n => new NoteDto
                        {
                            Id = n.Id,
                            TaskItemId = n.TaskItemId,
                            Note = n.Note,
                            CreatedDate = n.CreatedDate,
                            CreatedUser = n.CreatedUserId,
                        }
                        ).ToList(),
                        CurrentStatus = taskItem.TaskItemStatusHistories.OrderByDescending(x => x.CreatedUserId)
                        .Select(n => new StatusDto
                        {
                            Id = n.Status.Id,
                            Name = n.Status.Name,
                            Code = n.Status.Code,

                        }).FirstOrDefault(),
                        CreatedDate = taskItem.CreatedDate,
                        CreatedUserId = taskItem.CreatedUserId,
                    };
                    _logger.LogInformation($"Returning the newly deleted task {taskId} for user {userId}");
                    return Ok(deleteTask);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete task by id process failed: {ex.Message}");
                return StatusCode(500);
            }
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
                TaskItem child = sub.TaskItem;

                //Get the latest status history of a subtask and sort by created date
                TaskItemStatusHistory? latestStatus = child.TaskItemStatusHistories
                                        .OrderByDescending(c => c.CreatedDate)
                                        .FirstOrDefault();
                //Check if the latest status is not complete (StatusId != CompleteId)
                bool inCompletedChild = latestStatus!.StatusId != _statusChange.CompleteId;

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
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating StatusChangeComplete method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
                        return StatusCode(403);
                    }

                    TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
                    if (taskItem is null)
                    {
                        _logger.LogWarning($"TaskId {taskId} not found for UserId {userId}");
                        return NotFound(taskId);
                    }

                    // Prevent completing a task when any child SubTask is not complete.
                    if (HasIncompletedDescendants(taskItem))
                    {
                        return BadRequest("Cannot complete parent task with incomplete child sub-tasks.");
                    }

                    //add new status history for Complete
                    //Reuest DTO
                    TaskItemStatusHistory newTaskStatus = new TaskItemStatusHistory
                    {
                        TaskItemId = taskItem.Id,
                        StatusId = _statusChange.CompleteId,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = userId,
                    };


                    taskItem.TaskItemStatusHistories.Add(newTaskStatus);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Status Change to Complete is Successfull for userId {userId}");

                    //Response DTO
                    TaskDto statusResult = new TaskDto
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
                    _logger.LogInformation($"Status changed to Complete result for TaskId {taskItem.Id} and UserId {userId}");
                    _logger.LogInformation("Returning the status changed to complete result");
                    return CreatedAtAction(nameof(StatusChangeComplete), new { taskId = newTaskStatus.Id }, statusResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change status to complete process failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        //private bool HasStatusOfPending (TaskItem task) 
        //{
        //    if(task.TaskItemStatusHistories == task.TaskItemStatusHistories)
        //    {
        //        return true;
        //    }
        //    return false; 
        //}

        [HttpPost("{taskId}/status-change/pending", Name = "StatusChangePending")]
        public async Task<ActionResult<TaskDto>> StatusChangePending([FromHeader] Guid userId, Guid taskId)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating StatusChangePending method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
                        return StatusCode(403);
                    }

                    TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
                    if (taskItem is null)
                    {
                        _logger.LogWarning($"TaskId {taskId} not found for UserId {userId}");
                        return NotFound(taskId);
                    }
                    var latestStatus = taskItem.TaskItemStatusHistories
                        .OrderByDescending(s => s.CreatedDate)
                        .FirstOrDefault();

                    if (latestStatus != null && latestStatus.StatusId != _statusChange.CompleteId)
                    {
                        return StatusCode(403);
                    }

                    //add new status history for Complete
                    //Reuest DTO
                    TaskItemStatusHistory newTaskStatus = new TaskItemStatusHistory
                    {
                        TaskItemId = taskItem.Id,
                        StatusId = _statusChange.PendingId,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = userId,
                    };


                    taskItem.TaskItemStatusHistories.Add(newTaskStatus);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Status Change to Pending is Successfull for userId {userId}");

                    //Response DTO
                    TaskDto statusResult = new TaskDto
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
                            Id = _statusChange.PendingId,
                            Name = _statusChange.Pending,
                            Code = _statusChange.Code1
                        },

                        CreatedDate = taskItem.CreatedDate,
                        CreatedUserId = taskItem.CreatedUserId,
                    };
                    _logger.LogInformation($"Status changed to Pending result for TaskId {taskItem.Id} and UserId {userId}");
                    _logger.LogInformation("Returning the status changed to pending result");
                    return CreatedAtAction(nameof(StatusChangePending), new { taskId = newTaskStatus.Id }, statusResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Pending Status Change Process failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPut("{taskId}", Name = "EditTask")]
        public async Task<ActionResult<TaskDto>> EditTask([FromHeader] Guid userId, Guid taskId, TaskDto updateTaskDto)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating EditTask method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
                        return StatusCode(403);
                    }

                    TaskItem? taskItem = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId, userId);
                    if (taskItem is null)
                    {
                        _logger.LogWarning($"TaskId {taskId} not found for UserId {userId}");
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
                    if (updateTaskDto.ParentTaskId.HasValue) {
                        //remove old parent task relationship if there exists one
                        SubTask? temp = taskItem.SubTaskSubTaskItems.FirstOrDefault();
                        if (temp != null)
                        {
                            await _unitOfWork.TaskItem.DeleteSubTask(temp);
                        }
                    
                        //create relationship with provided parent task
                        SubTask? subTask = new()
                        {
                            TaskItemId = updateTaskDto.ParentTaskId.Value,
                            SubTaskItemId = taskItem.Id,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = userId
                        };
                        taskItem.SubTaskSubTaskItems.Add(subTask);
                        await _unitOfWork.SaveChangesAsync();
                    }
                

                    if (updateTaskDto.Title != null && updateTaskDto.Priority >= 0 )
                    {
                        taskItem.Title = updateTaskDto.Title;
                        taskItem.Priority = updateTaskDto.Priority;
                        if (updateTaskDto.DueDate != null)
                        {
                            taskItem.DueDate = updateTaskDto.DueDate.Value;
                        }
                        else
                        {
                            taskItem.DueDate = null;
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Task Edit is Successfull for userId {userId}");

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
                            CreatedDate = history.CreatedDate
                        }).First(),

                        CreatedDate = taskItem.CreatedDate,
                        CreatedUserId = taskItem.CreatedUserId
                    };
                    _logger.LogInformation($"Edited task result for TaskId {taskItem.Id} and UserId {userId}");
                    _logger.LogInformation("Returning the edited task result");
                    return Ok(editTaskResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Edit task process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
