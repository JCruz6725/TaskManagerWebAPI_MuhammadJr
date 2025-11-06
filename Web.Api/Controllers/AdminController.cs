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
        private int numUsers = 4;
        private DummyUser Alex = new DummyUser()
        {
            FirstName = "Alex",
            LastName = "Farmer",
            Email = "AFarmer@email.com",
            Password = "12345",
            TaskList = ["Exercise"],
            Tasks = [
                new DummyUser.TaskStruct(){
                    Title = "Run",
                    Priority = 10,
                    NumNotes = 1,
                    Notes = new List<string> {"Marathon"},
                    NumSubTasks = 2,
                    SubTasks = new List<string> {"Buy shoes", "Go to park"},
                    SubTasksPriorities = new List<int> {2, 5},
                    AssosciatedList = "Exercise"
                },
                new DummyUser.TaskStruct(){
                    Title = "Walk",
                    Priority = 20,
                    NumNotes = 1,
                    Notes = new List<string> {"Take dogs on walk"},
                    NumSubTasks = 1,
                    SubTasks = new List<string> {"Go to park"},
                    SubTasksPriorities = new List<int> {18}
                }
            ]
        };
        private DummyUser Jessie = new DummyUser()
        {
            FirstName = "Jessie",
            LastName = "Hopkins",
            Email = "JHopkins@email.com",
            Password = "password",
            Tasks = [
                new DummyUser.TaskStruct(){
                    Title = "Cook",
                    Priority = 30,
                    NumNotes = 2,
                    Notes = new List<string> {"spaghetti", "tacos"},
                    NumSubTasks = 3,
                    SubTasks = new List<string> {"Buy ingredients", "Chop veggies", "Wash dishes"},
                    SubTasksPriorities = new List<int> {5, 10, 20}
                }
            ]
        };
        private DummyUser April = new DummyUser()
        {
            FirstName = "April",
            LastName = "Rice",
            Email = "ARice@email.com",
            Password = "ARice",
            TaskList = ["Work"],
            Tasks = [
                new DummyUser.TaskStruct(){
                    Title = "Clean",
                    Priority = 40,
                    NumNotes = 1,
                    Notes = new List<string> {"Bathroom"},
                    NumSubTasks = 2,
                    SubTasks = new List<string> {"Clean kitchen", "Clean bathroom"},
                    SubTasksPriorities = new List<int> {20, 30}
                }
            ]
        };
        private DummyUser Niko = new DummyUser()
        {
            FirstName = "Niko",
            LastName = "Logan",
            Email = "Nlogan@email.com",
            Password = "abc",
            Tasks = [],
            TaskList = ["New list"]
        };



        private readonly StatusChange statusChange;
        private readonly TaskManagerAppDBContext context;
        private readonly ILogger<AdminController> logger;


        public AdminController(IOptions<StatusChange> statusChangeOptions, TaskManagerAppDBContext context, ILogger<AdminController> logger)
        {
            statusChange = statusChangeOptions.Value;
            this.context = context;
            this.logger = logger;
        }

        
        [HttpPost("/AddStatus", Name = "AddStatus")]
        public async Task<ActionResult<string>> AddStatus()
        {
            Status pendingStatus = new() { Id = statusChange.PendingId, Name = statusChange.Pending, Code = statusChange.Code1 };
            Status completedStatus = new() { Id = statusChange.CompleteId, Name = statusChange.Complete, Code = statusChange.Code2 };

            context.Add(pendingStatus);
            context.Add(completedStatus);

            await context.SaveChangesAsync();
            logger.LogInformation("Pending and completed status saved to database");
            return Ok("Status' Added");
        }
        

        [HttpPost("/AddDummyData", Name = "AddDummyData")]
        public async Task<ActionResult<string>> AddDummyData() 
        {
            DummyUser[] DummyUsers = { Alex, Jessie, April, Niko };

            //CREATE USERS, TASKS, NOTES, & SUB-TASKS (For each user)
            User user;
            TaskItem task;
            TaskItemNote note;
            SubTask subTask;
            List list;
            TaskWithinList taskWithinList;
            List<List> createdLists = new List<List>(); //used to keep track of the lists we created already


            //Create multiple users
            for (int userIndex = 0; userIndex < numUsers; userIndex++) { 
                DummyUser currDummyUser = DummyUsers[userIndex];

                //new user from dummy data
                user = new User
                {
                    FirstName = currDummyUser.FirstName,
                    LastName = currDummyUser.LastName,
                    Email = currDummyUser.Password,
                    Password = currDummyUser.Password,
                    CreatedDate = DateTime.Now,
                };
                context.Add(user);
                await context.SaveChangesAsync();
                user = await context.Users.FirstOrDefaultAsync(ui => ui.Id == user.Id);
                logger.LogInformation($"Dummy user {user!.FirstName} created and saved to database");



                //create list(s) for each user
                for (int listIndex = 0; listIndex < currDummyUser.TaskList.Count; listIndex++)
                {
                    string currDummyListItem = currDummyUser.TaskList[listIndex];

                    //new list from dummy data
                    list = new List()
                    {
                        Name = currDummyListItem,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = user.Id
                    };
                    context.Add(list);
                    await context.SaveChangesAsync();
                    list = context.Lists.Single(predicate: li => li.Id == list.Id);
                    createdLists.Add(list);
                    logger.LogInformation($"{user.FirstName}'s task #{listIndex + 1} created and saved");
                }
                logger.LogInformation($"All of {user.FirstName}'s lists created and saved");

                //create task(s), note(s), & subtask(s) for each user
                for (int taskIndex = 0; taskIndex < currDummyUser.Tasks.Count; taskIndex++) 
                {
                    DummyUser.TaskStruct currDummyTask = currDummyUser.Tasks[taskIndex];

                    //new task from dummy data
                    task = new TaskItem()
                    {
                        Title = currDummyTask.Title,
                        Priority = currDummyTask.Priority,
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
                    task = context.TaskItems.Single(predicate: ti => ti.Id == task.Id);
                    user.TaskItems.Add(task); //Add the task to the user
                    logger.LogInformation($"{user.FirstName}'s {task.Title} task created and saved");
                    

                    //create multiple note(s) for each task
                    for (int noteIndex = 0; noteIndex < currDummyTask.NumNotes; noteIndex++)
                    {   
                        //new note from dummy data
                        note = new TaskItemNote()
                        {
                            TaskItemId = task.Id,
                            Note = currDummyTask.Notes[noteIndex],
                            CreatedDate = DateTime.Now,
                            CreatedUserId = user.Id
                        };
                        context.Add(note);
                        await context.SaveChangesAsync();
                        task.TaskItemNotes.Add(note); //add note to task
                        logger.LogInformation($"{task.Title}'s note #{noteIndex+1} created and saved");
                    }
                    logger.LogInformation($"All of {user.FirstName}'s {task.Title} task notes created and saved");

                    
                    //create multiple subtask(s) for each task
                    for (int subTaskIndex = 0; subTaskIndex < currDummyTask.SubTasks.Count; subTaskIndex++)
                    {

                        //new subtask from dummy data
                        subTask = new SubTask()
                        {
                            TaskItemId = task.Id,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = user.Id,
                            SubTaskItem = new TaskItem()
                            {
                                Title = currDummyTask.SubTasks[subTaskIndex],
                                Priority = currDummyTask.SubTasksPriorities[subTaskIndex],
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
                        await context.SaveChangesAsync();
                        task.SubTaskTaskItems.Add(subTask); //Add the subtask to the task
                        logger.LogInformation($"{task.Title}'s subtask {subTask.SubTaskItem.Title} created and saved");
                    }
                    logger.LogInformation($"All of {user.FirstName}'s {task.Title} task subTasks created and saved");


                    //for each created list, check if task is supposed to be a part of the list
                    bool listFound = false;
                    for (int listIndex = 0; listIndex < createdLists.Count; listIndex++)
                    {
                        List currDummyList = createdLists[listIndex];

                        //adding task to list if it is associated together
                        if (currDummyTask.AssosciatedList == currDummyList.Name)
                        { 
                            //create new taskWithinList item using task just created
                            taskWithinList = new TaskWithinList()
                            {
                                TaskListId = currDummyList.Id,
                                TaskItemId = task.Id,
                                CreatedDate = DateTime.Now,
                                CreatedUserId = user.Id,
                            };
                            //add taskItem to list
                            context.Add(taskWithinList);
                            await context.SaveChangesAsync();
                            currDummyList.TaskWithinLists.Add(taskWithinList);
                            listFound = true;
                            logger.LogInformation($"Adding and saving task {task.Title} to list {currDummyList.Name}");
                        }
                        if (listFound) break;
                    }
                }
            }

            await context.SaveChangesAsync();

            logger.LogInformation("All users added");
            
            return Ok("Completed");
        }
        
    }
}
