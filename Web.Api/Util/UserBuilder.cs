using Web.Api.Persistence.Models;

namespace Web.Api.Util
{
    /// <summary>
    /// Provides a fluent API for constructing a <see cref="User"/> object with associated lists, tasks, and
    /// relationships.
    /// </summary>
    /// <remarks>The <see cref="UserBuilder"/> class allows for the creation and configuration of a <see
    /// cref="User"/> object by adding lists, tasks, notes, statuses, and relationships between tasks. It supports
    /// chaining method calls to enable a fluent and expressive syntax for building complex user data structures. <para>
    /// Use the <see cref="GetFinalUser"/> method to retrieve the fully constructed <see cref="User"/> object after all
    /// desired configurations have been applied. </para> <para> This class enforces certain preconditions for method
    /// calls. For example, a list must be added before tasks can be added to it, and a task must exist before notes or
    /// statuses can be added. Exceptions are thrown if these preconditions are not met. </para></remarks>
    /// <param name="email"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <param name="pass"></param>
    /// <param name="userId"></param>
    public class UserBuilder(string email, string first, string last, string pass, Guid userId) { 
        
        private User user = new User(){
            Id = userId,
            CreatedDate = DateTime.Now,
            Email = email,
            FirstName = first,
            LastName = last,
            Password = pass,
        };

        private List? currentList = null;
        private List<TaskItem> taskItems = [];
        private TaskItem? currentTaskItem = null;

        /// <summary>
        /// Creates a new list and adds it to the queued user object. 
        /// The newly created list becomes the current working list that when '<see cref="UserBuilder.AddTask(string, Guid, int, Guid, Guid)"/>' is call said task will be appended to the working list.
        /// </summary>
        /// <param name="listname"> The name/Display name of the list.</param>
        /// <param name="listId"> Unique identifier for the list object.</param>
        /// <returns>The current <see cref="UserBuilder"/> instance.</returns>
        public UserBuilder AddList(string listname, Guid listId) {
            List list = new() {
                Id = listId,
                CreatedDate = DateTime.Now,
                CreatedUser = user,
                Name = listname,

            };

            user.Lists.Add(list);
            currentList = list;
            return this;    
        }

        /// <summary>
        /// Adds a new task to the current working list with the specified details from params.
        /// </summary>
        /// <remarks>This method adds a task to the current working list of a user. A
        /// status history entry is also created with this task as and initial status.  <para> Before calling this method, ensure that a list has been added using the
        /// '<see cref="AddList"/>'. If no list is present, an exception will be thrown. </para></remarks>
        /// <param name="taskname"></param>
        /// <param name="statusId">The unique identifier of the status.</param>
        /// <param name="priority"></param>
        /// <param name="taskId">The unique identifier for the task being added.</param>
        /// <param name="taskItemStatusHistoryId">The unique identifier for the task's status history entry.</param>
        /// <returns>The current <see cref="UserBuilder"/> instance.</returns>
        /// <exception cref="Exception">Thrown if no list has been added prior to calling this method.</exception>
        public UserBuilder AddTask(string taskname, Guid statusId, int priority, Guid taskId, Guid taskItemStatusHistoryId) {
            if (currentList == null) 
            { 
                throw new Exception("Must add list prior to adding a task item."); 
            }

            TaskItem taskItem = new() {
                Id = taskId,
                CreatedDate = DateTime.Now,
                CreatedUser = user,
                Title = taskname,
                Priority = priority,
                TaskItemStatusHistories = [
                    new TaskItemStatusHistory(){
                        Id = taskItemStatusHistoryId,
                        StatusId = statusId,
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


        public UserBuilder AddOrphanTask(string taskname, Guid statusId, int priority, Guid taskId, Guid taskItemStatusHistoryId) {
            TaskItem taskItem = new() {
                Id = taskId,
                CreatedDate = DateTime.Now,
                CreatedUser = user,
                Title = taskname,
                Priority = priority,
                TaskItemStatusHistories = [
                    new TaskItemStatusHistory(){
                        Id= taskItemStatusHistoryId,
                        StatusId = statusId,
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

        /// <summary>
        /// Adds a note to the current task item.
        /// </summary>
        /// <remarks>A task item must be created before calling this method. If no task item exists, an
        /// exception is thrown.</remarks>
        /// <param name="content">The content of the note to add.</param>
        /// <param name="noteId">The unique identifier for the note.</param>
        /// <returns>The current <see cref="UserBuilder"/> instance.</returns>
        /// <exception cref="Exception">Thrown if no task item has been created prior to calling this method.</exception>
        public UserBuilder AddNote(string content, Guid noteId) {
            if (currentTaskItem is null) 
            {
                throw new Exception("Must create a task item first.");
            }

            currentTaskItem.TaskItemNotes.Add(
                new TaskItemNote() { 
                    Id= noteId,
                    CreatedUser = user,
                    CreatedDate= DateTime.Now,
                    Note = content
                }
            );
            return this;
        }

        /// <summary>
        /// Adds a status to the current task item.
        /// </summary>
        /// <remarks>This method associates a new status with the current task item by creating a
        /// corresponding task item status history entry. The task item must be created before calling this
        /// method.</remarks>
        /// <param name="statusId">The unique identifier of the status to add.</param>
        /// <param name="taskItemStatusHistoryId">The unique identifier for the task item status history entry.</param>
        /// <returns>The current <see cref="UserBuilder"/> instance.</returns>
        /// <exception cref="Exception">Thrown if the current task item has not been created.</exception>
        public UserBuilder AddStatus(Guid statusId, Guid taskItemStatusHistoryId) 
        {
            if(currentTaskItem is null)
            {
                throw new Exception("Must create a task item first.");
            }
            currentTaskItem.TaskItemStatusHistories.Add(
                new TaskItemStatusHistory() { 
                    Id = taskItemStatusHistoryId,
                    CreatedUser = user,
                    CreatedDate= DateTime.Now,
                    StatusId = statusId
                }
            );
            return this;
        }

        /// <summary>
        /// Adds a subtask relationship between a parent task and a child task.
        /// </summary>
        /// <remarks>This method establishes a subtask relationship by linking a parent task to a child
        /// task. Both tasks must exist in the specified lists, and their titles must match the provided
        /// values.</remarks>
        /// <param name="listnameP">The name of the list containing the parent task.</param>
        /// <param name="parent">The title of the parent task.</param>
        /// <param name="listnameC">The name of the list containing the child task.</param>
        /// <param name="child">The title of the child task.</param>
        /// <param name="subTaskId">The unique identifier for the subtask relationship.</param>
        /// <returns>The current <see cref="UserBuilder"/> instance.</returns>
        public UserBuilder LinkTasks(string listnameP, string parent, string listnameC, string child, Guid subTaskId) {
            user.SubTasks.Add(
                new SubTask() { 
                    Id= subTaskId,
                    CreatedUser = user,
                    CreatedDate = DateTime.Now,
                    TaskItem = user.Lists.Single(l => l.Name == listnameP).TaskWithinLists.Single(ti => ti.TaskItem.Title == parent ).TaskItem,
                    SubTaskItem = user.Lists.Single(l => l.Name == listnameC).TaskWithinLists.Single(ti => ti.TaskItem.Title == child ).TaskItem,
                }
            );
            return this;
        }
        
        /// <summary>
        /// Links a child task to a parent task within the current task list.
        /// </summary>
        /// <remarks>This method establishes a relationship between two tasks in the current working task list by
        /// designating one as the parent and the other as the child. The parent and child tasks must already exist in
        /// the current task list.</remarks>
        /// <param name="parent">The title of the parent task to which the child task will be linked.</param>
        /// <param name="child">The title of the child task that will be linked to the parent task.</param>
        /// <returns>The current <see cref="UserBuilder"/> instance.</returns>
        /// <exception cref="Exception">Thrown if no task list has been added prior to calling this method.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parent" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="child" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">If the the list contains more than one element of the same name.</exception>
        public UserBuilder LinkTasks(string parent, string child) {
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

        /// <summary>
        /// Links an orphaned task, task with no list, as a subtask to a parent task.
        /// </summary>
        /// <remarks>This method associates an existing task as a subtask of another task. Both the parent
        /// and child tasks must already exist and must not be apart of any list</remarks>
        /// <param name="parent">The title of the parent task to which the subtask will be linked. Must match an existing task title.</param>
        /// <param name="child">The title of the orphaned task to be linked as a subtask. Must match an existing task title.</param>
        /// <returns>The current <see cref="UserBuilder"/> instance.</returns>
        public UserBuilder LinkOrphanTasks(string parent, string child) {
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

        /// <summary>
        /// Retrieves the final user built by the <see cref="UserBuilder"/>. 
        /// </summary>
        /// <returns>The <see cref="User"/> object representing the final user.</returns>
        public User GetFinalUser() { 
            return user;    
        }
    }
}
