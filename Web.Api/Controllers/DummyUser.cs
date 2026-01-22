namespace Web.Api.Controllers
{
    public class DummyUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        public List<string> TaskList = new List<string>();

        public struct TaskStruct
        {
            public TaskStruct() { }
            public required string Title { get; set; }
            public required int Priority { get; set; }
            public required int NumNotes { get; set; }
            public required List<string> Notes { get; set; }
            public required int NumSubTasks { get; set; }
            public required List<string> SubTasks { get; set; }
            public required List<int> SubTasksPriorities { get; set; }
            public string AssociatedList { get; set; } = null!;
        }
        public required List<TaskStruct> Tasks = [];

    }
}
