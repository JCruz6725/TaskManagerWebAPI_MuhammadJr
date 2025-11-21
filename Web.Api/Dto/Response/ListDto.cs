namespace Web.Api.Dto.Response
{
    public class ListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TaskDto[] TaskItems { get; set; } = [];
        public DateTime CreatedDate { get; set; }
        public Guid CreatedUserId { get; set; }
    }
}
