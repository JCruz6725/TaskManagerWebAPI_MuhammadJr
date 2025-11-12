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
        const int DEFAULT_PRIORITY = 5;


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

            context.AddRange([
                GetUser1(), 
                GetUser2()
            ]);

            await context.SaveChangesAsync();
            return Ok("Dummy Data Added");
        }

        private User GetUser1() { 
            User user = new User() 
            { 
                CreatedDate = DateTime.Now,
                FirstName = "Alex",
                LastName = "Farmer",
                Email = "AFarmer@email.com",
                Password = "12345",
            };


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

            return user;
        }

        private User GetUser2() { 
            User user = new User() 
            { 
                CreatedDate = DateTime.Now.AddDays(-30.00),
                FirstName = "John",
                LastName = "Smith",
                Email = "John.Smith@mail.com",
                Password = "12345",
            };

            user.Lists = [
                new List()
                {
                    CreatedDate = DateTime.Now.AddDays(-4.00),
                    Name = "House construction project",
                    TaskWithinLists = [
                        new TaskWithinList()
                        { 
                            CreatedDate = DateTime.Now.AddDays(-4.00),
                            CreatedUser = user,
                            TaskItem =  new TaskItem()
                            {
                                CreatedUser = user,
                                Title = "Buy dry wall",
                                Priority = DEFAULT_PRIORITY,
                                CreatedDate = DateTime.Now,
                                TaskItemStatusHistories = [
                                    new TaskItemStatusHistory()
                                    {
                                        StatusId = statusChange.PendingId,
                                        CreatedDate = DateTime.Now.AddDays(-4.00),
                                        CreatedUser = user
                                    },
                                    new TaskItemStatusHistory()
                                    {
                                        StatusId = statusChange.CompleteId,
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
                                Title = "Paint drywall",
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
                                Title = "Add dry wall texture",
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
                                Title = "Buy paint",
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
                                Title = "Hang dry wall",
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
                },
                new List(){ 
                    CreatedDate= DateTime.Now,
                    CreatedUser = user,
                    Name = "Other list",
                    TaskWithinLists = [
                        new TaskWithinList(){ 
                            CreatedDate = DateTime.Now,
                            CreatedUser= user,
                            TaskItem = new TaskItem() { 
                                CreatedUser = user,
                                CreatedDate = DateTime.Now,
                                Title = "Some task item",
                                Priority = DEFAULT_PRIORITY,
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
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Buy dry wall").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Paint drywall").TaskItem,
                },
                new SubTask()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user,
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Paint drywall").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Add dry wall texture").TaskItem,
                },
                new SubTask()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user,
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Add dry wall texture").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Buy paint").TaskItem,
                },
                new SubTask()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user,
                    TaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Buy paint").TaskItem,
                    SubTaskItem = user.Lists.First().TaskWithinLists.Single(i => i.TaskItem.Title == "Hang dry wall").TaskItem,
                },
            ];

            return user;
        }
    }
}
