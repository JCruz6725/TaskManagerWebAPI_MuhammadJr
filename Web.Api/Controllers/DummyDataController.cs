using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using Web.Api.Dto.Request;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DummyDataController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;
        private readonly StatusChange statusChange;

        int numUsers = 5;
        private static readonly string[] FirstNames = { "Alex", "Jessie", "April", "Niko", "Brian" };
        private static readonly string[] LastNames = { "Farmer", "Hopkins", "Rice", "Logan", "Travis" };
        private static readonly string[] Emails = { "AFarmer@email.com", "JHopkins@email.com", "ARice@email.com", "NLogan@email.com", "BTravis@email.com" };
        private static readonly string[] Passwords = { "12345", "pasword", "ARice", "abc", "BrianT" };
        private List<Guid> Ids { get; set; } = [];

        int numTasks = 4;
        private static readonly string[] TaskTitle = { "Run", "Walk", "Cook", "Clean" };
        private static readonly int[] TaskPriority = { 0, 1, 2, 3 };

        int numNotes = 4;
        private static readonly string[] Notes = { "Marathon", "Take dogs on a walk", "Spaghetti", "Bathroom" };

        int numSubTasks = 3;
        private static readonly string[,] SubTasks =
        {
            { "Buy shoes", "Go to park", null },
            { "Go to park", null, null },
            { "Buy ingredients", "Wash dishes", "Chop veggies" },
            { "Clean kitchen", "Clean bathroom", null }
        };


        private readonly TaskManagerAppDBContext _t;

        public DummyDataController(IOptions<StatusChange> statusChangeOptions, TaskManagerAppDBContext t)
        {
            this.unitOfWork = unitOfWork;
            statusChange = statusChangeOptions.Value;
            _t = t;
        }

        
        [HttpPost("/AddStatus", Name = "AddStatus")]
        public async Task<ActionResult<string>> AddStatus()
        {
            Status pendingStatus = new Status() { Id = statusChange.PendingId, Name = "Pending", Code = 1 };
            Status completedStatus = new Status() { Id = statusChange.CompleteId, Name = "Complete", Code = 2 };

            _t.Add(pendingStatus);
            _t.Add(completedStatus);

            await _t.SaveChangesAsync();
            return Ok("Status Added");
        }
        

        [HttpPost(Name = "FillDatabase")]
        public async Task<ActionResult<string>> FillDatabase() //add a return
        {


            //Fill Database with status'

            
            //CREATE TASKS, NOTES, & SUB-TASKS (For each user)
            User user;
            TaskItem task;
            TaskItemNote note;
            SubTask subTask;
            for (int userIndex = 0; userIndex < numUsers; userIndex++) { //for every hardcoded user

                //create new user
                user = new User
                {
                    FirstName = FirstNames[userIndex],
                    LastName = LastNames[userIndex],
                    Email = Emails[userIndex],
                    Password = Passwords[userIndex],
                    CreatedDate = DateTime.Now,
                };

                await unitOfWork.User.CreateUserAsync(user);
                await unitOfWork.SaveChangesAsync();
                user = await unitOfWork.User.GetUserByIdAsync(user.Id);

                for (int taskIndex = 0; taskIndex < numTasks; taskIndex++) //create multiple tasks
                {
                    //create new task
                    task = new TaskItem()
                    {
                        CreatedUserId = user.Id,
                        Title = TaskTitle[taskIndex],
                        Priority = TaskPriority[taskIndex],
                        CreatedDate = DateTime.Now,
                        DueDate = DateTime.Now,
                        TaskItemStatusHistories = [
                            new TaskItemStatusHistory() {
                                StatusId = statusChange.PendingId, 
                                CreatedDate = DateTime.Now, 
                                CreatedUserId = user.Id
                            }
                        ]
                    };
                    await unitOfWork.TaskItem.CreateTaskAsync(task);
                    await unitOfWork.SaveChangesAsync();

                    task = await unitOfWork.TaskItem.GetTaskByIdAsync(task.Id);
                    //taskIds.Add(task.Id);


                    //New note from hardcoded data
                    note = new TaskItemNote()
                    {
                        TaskItemId = task.Id,
                        Note = Notes[taskIndex],
                        CreatedDate = DateTime.Now,
                        CreatedUserId = user.Id//userIds[taskIndex]
                    };
                    await unitOfWork.TaskItem.CreateNoteAsync(note);
                    
                    /*
                    //multiple new subtasks for each task
                    for (int subTaskIndex = 0; subTaskIndex < numSubTasks; subTaskIndex++)
                    {
                        subTask = new SubTask()
                        {
                            TaskItemId = task.Id,
                            CreatedDate = DateTime.Now,
                            CreatedUserId
                        };
                    }*/
                }
            }


            /*
            //REGISTER USERS
            User users;
            for (int i = 0; i < numUsers; i++) {
                users = new User
                {
                    FirstName = FirstNames[i],
                    LastName = LastNames[i],
                    Email = Emails[i],
                    Password = Passwords[i],
                    CreatedDate = DateTime.Now,
                };
                

                await unitOfWork.User.CreateUserAsync(users);
                await unitOfWork.SaveChangesAsync();

                Ids.Add(users.Id);
            }

            
            //CREATE TASKS (For each user)
            
            for (int userIndex = 0; userIndex < numUsers; userIndex++) { //for every hardcoded user
                for (int taskIndex = 0; taskIndex < numTasks; taskIndex++) //create multiple tasks
                {
                    TaskItem task = new TaskItem()
                    {
                        CreatedUserId = Ids[userIndex],
                        Title = Title[taskIndex],
                        Priority = Priority[taskIndex],
                        CreatedDate = DateTime.Now,
                        DueDate = DateTime.Now,
                        TaskItemStatusHistories = [
                            new TaskItemStatusHistory() {
                                StatusId = statusChange.PendingId, 
                                CreatedDate = DateTime.Now, 
                                CreatedUserId = Ids[userIndex] 
                            }
                        ]
                    };
                    await unitOfWork.TaskItem.CreateTaskAsync(task);
                    await unitOfWork.SaveChangesAsync();
                    task = await unitOfWork.TaskItem.GetTaskByIdAsync(task.Id);
                }
            }

            //CREATE NOTES (For each user)
            //TaskItemNote noteCreation;
            */
            await unitOfWork.SaveChangesAsync();
            
            return Ok("Completed");
        }
        
    }
}
