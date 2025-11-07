using Web.Api.Persistence.Models;

namespace Web.Api.Dto.Response
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
        public Guid ParentTaskId { get; set; }
        public List<NoteDto> Notes { get; set; } = [];
        public StatusDto? CurrentStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedUserId { get; set; }
    }
}
