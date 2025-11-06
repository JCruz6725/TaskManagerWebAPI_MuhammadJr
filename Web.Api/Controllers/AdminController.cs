using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog.Filters;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
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
        public async Task<ActionResult<string>> AddDummyData() { 
        const int DEFAULT_PRIORITY = 5;
            


            User user = new User() 
            { 
                CreatedDate = DateTime.Now,
                FirstName = "Alex",
                LastName = "Farmer",
                Email = "AFarmer@email.com",
                Password = "12345",
            };

            context.Add(user);

            user.Lists = [
                new List()
                {
                    CreatedDate = DateTime.Now,
                    Name = "Mustang project",
                    TaskWithinLists = [
                        new TaskWithinList()
                        { 
                            CreatedDate = DateTime.Now,
                            CreatedUser = user,
                            TaskItem =  new TaskItem()
                            {
                                CreatedUser = user,
                                Title = "Test Drive",
                                Priority = DEFAULT_PRIORITY,
                                CreatedDate = DateTime.Now,
                                TaskItemNotes = [
                                    new TaskItemNote() 
                                    {
                                        CreatedUser = user,
                                        CreatedDate = DateTime.Now,
                                        Note = "Destination, the mall. Far enough to satisfy the test drive."
                                    }  
                                ],
                                TaskItemStatusHistories = [
                                    new TaskItemStatusHistory()
                                    {
                                        StatusId = statusChange.PendingId,
                                        CreatedDate = DateTime.Now,
                                        CreatedUser = user
                                    }
                                ]
                            }
                        },
                        new TaskWithinList()
                        { 
                            CreatedDate = DateTime.Now,
                            CreatedUser = user,
                            TaskItem =  new TaskItem()
                            {
                                CreatedUser = user,
                                Title = "Fix radiator",
                                Priority = DEFAULT_PRIORITY,
                                CreatedDate = DateTime.Now,
                                TaskItemStatusHistories = [
                                    new TaskItemStatusHistory()
                                    {
                                        StatusId = statusChange.PendingId,
                                        CreatedDate = DateTime.Now,
                                        CreatedUser = user
                                    }
                                ]
                            }
                        },
                        new TaskWithinList()
                        { 
                            CreatedDate = DateTime.Now,
                            CreatedUser = user,
                            TaskItem =  new TaskItem()
                            {
                                CreatedUser = user,
                                Title = "Add water",
                                Priority = DEFAULT_PRIORITY,
                                CreatedDate = DateTime.Now,
                                TaskItemStatusHistories = [
                                    new TaskItemStatusHistory()
                                    {
                                        StatusId = statusChange.PendingId,
                                        CreatedDate = DateTime.Now,
                                        CreatedUser = user
                                    }
                                ]
                            }
                        },
                        new TaskWithinList()
                        { 
                            CreatedDate = DateTime.Now,
                            CreatedUser = user,
                            TaskItem =  new TaskItem()
                            {
                                CreatedUser = user,
                                Title = "Check Oil",
                                Priority = DEFAULT_PRIORITY,
                                CreatedDate = DateTime.Now,
                                TaskItemStatusHistories = [
                                    new TaskItemStatusHistory()
                                    {
                                        StatusId = statusChange.PendingId,
                                        CreatedDate = DateTime.Now,
                                        CreatedUser = user
                                    }
                                ]
                            }
                        },
                        new TaskWithinList()
                        { 
                            CreatedDate = DateTime.Now,
                            CreatedUser = user,
                            TaskItem =  new TaskItem()
                            {
                                CreatedUser = user,
                                Title = "Check Tire Presure",
                                Priority = DEFAULT_PRIORITY,
                                CreatedDate = DateTime.Now,
                                TaskItemStatusHistories = [
                                    new TaskItemStatusHistory()
                                    {
                                        StatusId = statusChange.PendingId,
                                        CreatedDate = DateTime.Now,
                                        CreatedUser = user
                                    }
                                ]
                            }
                        }
                    ]
                } 
            ];

            user.SubTasks = [
                new SubTask()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user,
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Test Drive").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Check Tire Presure").TaskItem,
                },

                new SubTask()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user,
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Check Tire Presure").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Check Oil").TaskItem,
                },
                new SubTask()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user,
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Check Oil").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Add water").TaskItem,
                },
                new SubTask()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user,
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Add water").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Fix radiator").TaskItem,
                },
            ];
            await context.SaveChangesAsync();
            return Ok("Dummy Data Added");
        }
    }
}
