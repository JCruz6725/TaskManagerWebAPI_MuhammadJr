using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

namespace Web.Api.Controllers
{

    public class DummyUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        public struct TaskStruct
        { 
            public required string Title {  get; set; }
            public required int Priority { get; set; }
            public required int NumNotes { get; set; }
            public required List<string> Notes { get; set; }
            public required int NumSubTasks {  get; set; }
            public required List<string> SubTasks { get; set; }
        }
        public required List<TaskStruct> Tasks = new List<TaskStruct>();

    }


    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly Random random = new(); //used to create random priorities for subtasks
        private int numUsers = 4;
        private DummyUser Alex = new DummyUser()
        {
            FirstName = "Alex",
            LastName = "Farmer",
            Email = "AFarmer@email.com",
            Password = "12345",
            Tasks = [
                new DummyUser.TaskStruct(){
                    Title = "Run",
                    Priority = 10,
                    NumNotes = 1,
                    Notes = new List<string> {"Marathon"},
                    NumSubTasks = 2,
                    SubTasks = new List<string> {"Buy shoes", "Go to park"}
                },
                new DummyUser.TaskStruct(){
                    Title = "Walk",
                    Priority = 20,
                    NumNotes = 1,
                    Notes = new List<string> {"Take dogs on walk"},
                    NumSubTasks = 1,
                    SubTasks = new List<string> {"Go to park"}
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
                    SubTasks = new List<string> { "Buy ingredients", "Wash dishes", "Chop veggies" }
                }
            ]
        };
        private DummyUser April = new DummyUser()
        {
            FirstName = "April",
            LastName = "Rice",
            Email = "ARice@email.com",
            Password = "ARice",
            Tasks = [
                new DummyUser.TaskStruct(){
                    Title = "Clean",
                    Priority = 40,
                    NumNotes = 1,
                    Notes = new List<string> {"Bathroom"},
                    NumSubTasks = 2,
                    SubTasks = new List<string> { "Clean kitchen", "Clean bathroom" }
                }
            ]
        };
        private DummyUser Niko = new DummyUser()
        {
            FirstName = "Niko",
            LastName = "Logan",
            Email = "Nlogan@email.com",
            Password = "abc",
            Tasks = []
        };




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
            Status pendingStatus = new() { Id = statusChange.PendingId, Name = statusChange.Pending, Code = statusChange.Code1 };
            Status completedStatus = new() { Id = statusChange.CompleteId, Name = statusChange.Complete, Code = statusChange.Code2 };

            context.Add(pendingStatus);
            context.Add(completedStatus);

            await context.SaveChangesAsync();
            return Ok("Status Added");
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
            List<List> tempListsCollection;


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

                
                /*
                //create list(s) for each user
                tempListsCollection = new List<List>();
                for (int listIndex = 0; listIndex < ; listIndex++)
                {
                    if (DummyLists[userIndex, listIndex] != null) //check if there is dummy data to be created
                    {
                        list = new List()
                        {
                            Name = DummyLists[userIndex,listIndex]!,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = user.Id
                        };
                        context.Add(list);
                        await context.SaveChangesAsync();
                        list = await context.Lists.FirstOrDefaultAsync(li => li.Id == list.Id);
                        tempListsCollection.Add(list);
                    }
                }*/



                //int tempListIndex = 0;
                //int numTaskInList = 0;
                //create multiple lists, tasks, notes, & subtasks for each user
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
                    task = await context.TaskItems.Include(item => item.TaskItemNotes)
                                                  .Include(history => history.TaskItemStatusHistories)
                                                  .ThenInclude(stat => stat.Status)
                                                  .FirstOrDefaultAsync(ti => ti.Id == task.Id);
                    user.TaskItems.Add(task); //Add the task to the user
                    
                    
                    //create multiple note(s) for each task
                    for (int noteIndex = 0; noteIndex < currDummyTask.NumNotes; noteIndex++)
                    {
                        string currDummyNote = currDummyTask.Notes[noteIndex];
                        //new note from dummy data
                        note = new TaskItemNote()
                        {
                            TaskItemId = task.Id,
                            Note = currDummyNote,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = user.Id
                        };
                        context.Add(note);
                        await context.SaveChangesAsync();
                        task.TaskItemNotes.Add(note); //add note to task
                    }


                    
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
                                Priority = random.Next(task.Priority), //Generates random priority value between: 0 - parent task priority 
                                                                       //We want the priority of a subtask to be higher than the parent task (need to complete subtasks before parent task)
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
                        //task.SubTaskTaskItems.Add(subTask); //Add the subtask to the task

                    }

                    /*
                    //ADD THE TASK TO A LIST (If available)
                    if (tempListsCollection[tempListIndex] != null) {
                        //create new taskWithinList item using task just created
                        TaskWithinList taskWithinList = new TaskWithinList()
                        {
                            TaskListId = tempListsCollection[tempListIndex].Id,
                            TaskItemId = task.Id,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = user.Id,
                        };
                        //add taskItem to list
                        tempListsCollection[tempListIndex].TaskWithinLists.Add(taskWithinList);
                        numTaskInList++; //we just added a task to the list  
                    }

                    if (numTaskInList == 2)
                    {
                        tempListIndex++; //we only move to a new list in the tempCollection when each list has 2 tasks
                    }*/
                }
            }


           
            context.SaveChanges();
            
            return Ok("Completed");
        }
        
    }
}
