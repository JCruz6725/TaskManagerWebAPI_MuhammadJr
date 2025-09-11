namespace Web.Api.Dto.Response
{
    public class NoteDto
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedUser { get; set; }
    }
}
