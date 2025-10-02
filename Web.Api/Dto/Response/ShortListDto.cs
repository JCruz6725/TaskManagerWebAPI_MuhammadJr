namespace Web.Api.Dto.Response {
    public class ShortListDto {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedUserId { get; set; }
    }
}