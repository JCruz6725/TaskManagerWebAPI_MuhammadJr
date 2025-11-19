namespace Web.Api.Dto.Request
{
    public class TaskCreateDto
    {
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
        public Guid? ParentTaskId { get; set; } 

    }
}
