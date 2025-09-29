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
        public async Task<ActionResult<TaskDto>> GetTaskById(Guid taskId)
        {
            //var getId = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);
            //if (getId == null)
            //{
            //    return NotFound();
            //}
            //return Ok(getId);

            throw new NotImplementedException();
        }

        [HttpPost( Name = "CreateTask")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromHeader] Guid userId,[FromBody]TaskCreateDto taskCreatedDto)
        {
            var userExist = await _unitOfWork.User.GetUserByIdAsync(userId);  //Check if user exists before adding task
            if (userExist is null)
            {
                return NotFound("user account does not exist");
            }

            //Request DTO
            //create a new instance of TaskItem 
            //calls the TaskItem prop and set the task created dto to its prop
            var taskCreation = new TaskItem()
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
                CreatedDate = taskCreation.CreatedDate,
                CreatedUserId = taskCreation.CreatedUserId,
            };
            return CreatedAtAction(nameof(CreateTask),new {taskId = taskCreation.Id}, creationResult);
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
