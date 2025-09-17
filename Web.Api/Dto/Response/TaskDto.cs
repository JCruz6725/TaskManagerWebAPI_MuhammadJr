namespace Web.Api.Dto.Response
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string MyProperty { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        public NoteDto Notes { get; set; }
        public StatusDto CurrentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedUser { get; set; }
    }
}
