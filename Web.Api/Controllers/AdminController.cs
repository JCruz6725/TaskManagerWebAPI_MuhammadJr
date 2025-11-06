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
            TaskList = ["Exercise", "Random"],
            Tasks = [
                new DummyUser.TaskStruct(){
                    Title = "Run",
                    Priority = 10,
                    NumNotes = 1,
                    Notes = new List<string> {"Marathon"},
                    NumSubTasks = 2,
                    SubTasks = new List<string> {"Buy shoes", "Go to park"},
                    SubTasksPriorities = new List<int> {2, 5},
                    AssociatedList = "Exercise"
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
                logger.LogInformation($"Dummy user {user!.FirstName} created and saved to database");

      

                //create list(s) for each user
                for (int listIndex = 0; listIndex < currDummyUser.TaskList.Count; listIndex++)
                {
                    string currDummyListItem = currDummyUser.TaskList[listIndex];

                    //new list from dummy data
                    user.Lists.Add(
                        new List()
                        {
                            CreatedDate = DateTime.Now,
                            Name = currDummyListItem, 
                        }
                    );
                    await context.SaveChangesAsync();
                    logger.LogInformation($"{user.FirstName}'s task #{listIndex + 1} created and saved");
                }
                logger.LogInformation($"All of {user.FirstName}'s lists created and saved");



                //create task(s), note(s), & subtask(s) for each user
                for (int taskIndex = 0; taskIndex < currDummyUser.Tasks.Count; taskIndex++) 
                {
                    DummyUser.TaskStruct currDummyTask = currDummyUser.Tasks[taskIndex];

                    //new task from dummy data
                    user.TaskItems.Add(
                        new TaskItem()
                        {
                            Title = currDummyTask.Title,
                            Priority = currDummyTask.Priority,
                            CreatedDate = DateTime.Now,
                            DueDate = DateTime.Now,
                            TaskItemStatusHistories = [
                                new TaskItemStatusHistory() {
                                    StatusId = statusChange.PendingId,
                                    CreatedDate = DateTime.Now,
                                    CreatedUser = user
                                }
                            ]
                        }
                    );
                    await context.SaveChangesAsync();
                    task = user.TaskItems.ElementAt(taskIndex);
                    logger.LogInformation($"{user.FirstName}'s {task.Title} task created and saved");
                    


                    //create multiple note(s) for each task
                    for (int noteIndex = 0; noteIndex < currDummyTask.NumNotes; noteIndex++)
                    {
                        //new note from dummy data
                        task.TaskItemNotes.Add(
                            new TaskItemNote()
                            {
                                Note = currDummyTask.Notes[noteIndex],
                                CreatedDate = DateTime.Now,
                                CreatedUser = user
                            }
                        );
                        await context.SaveChangesAsync();
                        logger.LogInformation($"{task.Title}'s note #{noteIndex+1} created and saved");
                    }
                    logger.LogInformation($"All of {user.FirstName}'s {task.Title} task notes created and saved");

                    

                    //create multiple subtask(s) for each task
                    for (int subTaskIndex = 0; subTaskIndex < currDummyTask.SubTasks.Count; subTaskIndex++)
                    {

                        //new subtask from dummy data
                        task.SubTaskTaskItems.Add(
                            new SubTask()
                            {
                                CreatedDate = DateTime.Now,
                                CreatedUser = user,
                                SubTaskItem = new TaskItem()
                                {
                                    Title = currDummyTask.SubTasks[subTaskIndex],
                                    Priority = currDummyTask.SubTasksPriorities[subTaskIndex],
                                    CreatedDate = DateTime.Now,
                                    CreatedUser = user,
                                    DueDate = DateTime.Now,
                                    TaskItemStatusHistories = [
                                        new TaskItemStatusHistory() {
                                            StatusId = statusChange.PendingId,
                                            CreatedDate= DateTime.Now,
                                            CreatedUser = user
                                        }
                                    ]
                                }
                            }
                        );
                        /*
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
                        };*/
                        //context.Add(subTask);
                        await context.SaveChangesAsync();
                        //task.SubTaskTaskItems.Add(subTask); //Add the subtask to the task
                        subTask = task.SubTaskTaskItems.ElementAt(subTaskIndex);
                        logger.LogInformation($"{task.Title}'s subtask {subTask.SubTaskItem.Title} created and saved");
                    }
                    logger.LogInformation($"All of {user.FirstName}'s {task.Title} task subTasks created and saved");



                    if (currDummyTask.AssociatedList != null) //check if task belongs in a list
                    {
                        bool listFound = false;
                        //for each list in the user, check if task is supposed to be a part of that list
                        for (int listIndex = 0; listIndex < user.Lists.Count; listIndex++)
                        {
                            List currDummyList = user.Lists.ElementAt(listIndex);

                            //adding task to list if it is associated together
                            if (currDummyTask.AssociatedList == currDummyList.Name)
                            {
                                //create new taskWithinList item using task just created
                                currDummyList.TaskWithinLists = [
                                    new TaskWithinList()
                                    {
                                        CreatedUser = user,
                                        TaskItem = task,
                                        CreatedDate = DateTime.Now
                                    }
                                ];
                                await context.SaveChangesAsync();
                                listFound = true;
                                logger.LogInformation($"Adding and saving task {task.Title} to list {currDummyList.Name}");
                            }
                            if (listFound) break;
                        }
                    }

                }
                logger.LogInformation($"All of {user.FirstName}'s tasks created and saved");
            }
            logger.LogInformation("All users added");

            await context.SaveChangesAsync();

            return Ok("Completed");
        }
        
    }
}
