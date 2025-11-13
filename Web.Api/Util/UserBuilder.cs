using Web.Api.Persistence.Models;

namespace Web.Api.Util
{
    public class UserBuilder(string email, string first, string last, string pass) { 
        
        private User user = new User(){ 
            CreatedDate = DateTime.Now,
            Email = email,
            FirstName = first,
            LastName = last,
            Password = pass
        };

        private List? currentList = null;
        private List<TaskItem> taskItems = [];
        private TaskItem? currentTaskItem = null;


        public UserBuilder AddList(string listname) {
            List list = new() {
                CreatedDate = DateTime.Now,
                CreatedUser = user,
                Name = listname,
            };

            user.Lists.Add(list);
            currentList = list;
            return this;    
        }


        public UserBuilder AddTask(string taskname, Guid StatusPendingId) {
            if (currentList == null) 
            { 
                throw new Exception("Must add list prior to adding a task item."); 
            }

            TaskItem taskItem = new() {
                CreatedDate = DateTime.Now,
                CreatedUser = user,
                Title = taskname,
                TaskItemStatusHistories = [
                    new TaskItemStatusHistory(){
                        StatusId = StatusPendingId,
                        CreatedDate = DateTime.Now,
                        CreatedUser = user,
                    }
                ]
            };

            currentList.TaskWithinLists.Add(
                new TaskWithinList() { 
                    CreatedDate = DateTime.Now,
                    CreatedUser= user,
                    TaskItem = taskItem
                }
            );

            taskItems.Add(taskItem);
            currentTaskItem = taskItem;

            return this;    
        }


        public UserBuilder AddOrphanTask(string taskname, Guid StatusPendingId) {
            TaskItem taskItem = new() {
                CreatedDate = DateTime.Now,
                CreatedUser = user,
                Title = taskname,
                TaskItemStatusHistories = [
                    new TaskItemStatusHistory(){
                        StatusId = StatusPendingId,
                        CreatedDate = DateTime.Now,
                        CreatedUser = user,
                    }
                ]
            };

            user.TaskItems.Add(taskItem);

            taskItems.Add(taskItem);
            currentTaskItem = taskItem;

            return this;    
        }


        public UserBuilder AddNote(string content) {
            if (currentTaskItem is null) 
            {
                throw new Exception("Must create a task item first.");
            }

            currentTaskItem.TaskItemNotes.Add(
                new TaskItemNote() { 
                    CreatedUser = user,
                    CreatedDate= DateTime.Now,
                    Note = content
                }
            );
            return this;
        }



        public UserBuilder AddSubTask(string listnameP, string parent, string listnameC, string child) {
            user.SubTasks.Add(
                new SubTask() { 
                    CreatedUser = user,
                    CreatedDate = DateTime.Now,
                    TaskItem = user.Lists.Single(l => l.Name == listnameP).TaskWithinLists.Single(ti => ti.TaskItem.Title == parent ).TaskItem,
                    SubTaskItem = user.Lists.Single(l => l.Name == listnameC).TaskWithinLists.Single(ti => ti.TaskItem.Title == child ).TaskItem,
                }
            );
            return this;
        }
        

        public UserBuilder AddSubTask(string parent, string child) {
            if (currentList == null) 
            { 
                throw new Exception("Must add list prior to adding a task item."); 
            }

            user.SubTasks.Add(
                new SubTask() { 
                    CreatedUser = user,
                    CreatedDate = DateTime.Now,
                    TaskItem = currentList.TaskWithinLists.Single(ti => ti.TaskItem.Title == parent ).TaskItem,
                    SubTaskItem = currentList.TaskWithinLists.Single(ti => ti.TaskItem.Title == child ).TaskItem,
                }
            );
            return this;
        }

        public UserBuilder AddSubTaskForOrphan(string parent, string child) {
            user.SubTasks.Add(
                new SubTask() { 
                    CreatedUser = user,
                    CreatedDate = DateTime.Now,
                    TaskItem = taskItems.Single(ti => ti.Title == parent ),
                    SubTaskItem = taskItems.Single(ti => ti.Title == child ),
                }
            );
            return this;
        }


        public User GetFinalUser() { 
            return user;    
        }
    }
}
