using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        //HARD DATA
        private int numUsers = 5;
        private static readonly string[] DummyFirstNames = { "Alex", "Jessie", "April", "Niko", "Brian" };
        private static readonly string[] DummyLastNames = { "Farmer", "Hopkins", "Rice", "Logan", "Travis" };
        private static readonly string[] DummyEmails = { "AFarmer@email.com", "JHopkins@email.com", "ARice@email.com", "NLogan@email.com", "BTravis@email.com" };
        private static readonly string[] DummyPasswords = { "12345", "pasword", "ARice", "abc", "BrianT" };
        //private List<Guid> Ids { get; set; } = [];

        private int numTasks = 4;
        private static readonly string[] DummyTaskTitles = { "Run", "Walk", "Cook", "Clean" };
        private static readonly int[] DummyTaskPriorities = { 10, 20, 30, 40 };

        private int numNotes = 4;
        private static readonly string[] DummyNotes = { "Marathon", "Take dogs on a walk", "Spaghetti", "Bathroom" };

        private int numSubTasks = 3; //per task
        private static readonly string[,] DummySubTasks =
        {
            { "Buy shoes", "Go to park", null },
            { "Go to park", null, null },
            { "Buy ingredients", "Wash dishes", "Chop veggies" },
            { "Clean kitchen", "Clean bathroom", null }
        };
        private Random random = new Random(); //used to create random priorities for subtasks
        //


        private readonly StatusChange statusChange;
        private readonly TaskManagerAppDBContext context;


        public AdminController(IOptions<StatusChange> statusChangeOptions, TaskManagerAppDBContext context)
        {
            statusChange = statusChangeOptions.Value;
            this.context = context;
        }

        
        [HttpPost("/AddStatus", Name = "AddStatus")]
        public async Task<ActionResult<string>> AddStatus()
        {
            Status pendingStatus = new Status() { Id = statusChange.PendingId, Name = "Pending", Code = 1 };
            Status completedStatus = new Status() { Id = statusChange.CompleteId, Name = "Complete", Code = 2 };

            context.Add(pendingStatus);
            context.Add(completedStatus);

            await context.SaveChangesAsync();
            return Ok("Status Added");
        }
        

        [HttpPost("/AddDummyData", Name = "AddDummyData")]
        public async Task<ActionResult<string>> AddDummyData() 
        {
            //CREATE TASKS, NOTES, & SUB-TASKS (For each user)
            User user;
            TaskItem task;
            TaskItemNote note;
            SubTask subTask;

            //Create multiple users
            for (int userIndex = 0; userIndex < numUsers; userIndex++) { 

                //new user from dummy data
                user = new User
                {
                    FirstName = DummyFirstNames[userIndex],
                    LastName = DummyLastNames[userIndex],
                    Email = DummyEmails[userIndex],
                    Password = DummyPasswords[userIndex],
                    CreatedDate = DateTime.Now,
                };

                context.Add(user);
                await context.SaveChangesAsync();
                user = await context.Users.FirstOrDefaultAsync(ui => ui.Id == user.Id);

                //create multiple tasks, notes, & subtasks for each user
                for (int taskIndex = 0; taskIndex < numTasks; taskIndex++) 
                {
                    //new task from dummy data
                    task = new TaskItem()
                    {
                        Title = DummyTaskTitles[taskIndex],
                        Priority = DummyTaskPriorities[taskIndex],
                        CreatedDate = DateTime.Now,
                        CreatedUserId = user.Id,
                        DueDate = DateTime.Now,
                        TaskItemStatusHistories = [
                            new TaskItemStatusHistory() {
                                StatusId = statusChange.PendingId, 
                                CreatedDate = DateTime.Now, 
                                CreatedUserId = user.Id
                            }
                        ]
                    };
                    context.Add(task);
                    await context.SaveChangesAsync();
                    task = await context.TaskItems.Include(item => item.TaskItemNotes)
                                                  .Include(history => history.TaskItemStatusHistories)
                                                  .ThenInclude(stat => stat.Status)
                                                  .FirstOrDefaultAsync(ti => ti.Id == task.Id);


                    //new note from dummy data
                    note = new TaskItemNote()
                    {
                        TaskItemId = task.Id,
                        Note = DummyNotes[taskIndex],
                        CreatedDate = DateTime.Now,
                        CreatedUserId = user.Id//userIds[taskIndex]
                    };
                    context.Add(note);
                    context.SaveChanges();
                    
                    
                    //create multiple subtasks for each task
                    for (int subTaskIndex = 0; subTaskIndex < numSubTasks; subTaskIndex++)
                    {
                        if (DummySubTasks[taskIndex, subTaskIndex] != null) { //check if there is dummy data to create
                            //new subtask from dummy data
                            subTask = new SubTask()
                            {
                                TaskItemId = task.Id,
                                CreatedDate = DateTime.Now,
                                CreatedUserId = user.Id,
                                SubTaskItem = new TaskItem()
                                {
                                    Title = DummySubTasks[taskIndex, subTaskIndex],
                                    Priority = random.Next(task.Priority), //Generates random priority value between: 0 - parent task priority 
                                                                           //We don't want the priority of a parent task to be higher than the subtask (need to complete subtasks before parent task)
                                    CreatedDate = DateTime.Now,
                                    CreatedUserId = user.Id,
                                    DueDate = DateTime.Now,
                                    TaskItemStatusHistories = [
                                        new TaskItemStatusHistory() {
                                            StatusId = statusChange.PendingId,
                                            CreatedDate= DateTime.Now,
                                            CreatedUserId = user.Id,
                                        }
                                    ]
                                }
                            };
                            context.Add(subTask);
                            context.SaveChanges();
                        }
                        
                    }
                }
            }


           
            context.SaveChanges();
            
            return Ok("Completed");
        }
        
    }
}
