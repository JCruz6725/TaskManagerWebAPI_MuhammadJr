namespace Web.Api.Dto.Request
{
    public class RequireExtraInfoFlagDto
    {
        public bool RequiresExtraInfo { get; set; }

        public Guid UserId { get; set; }
    }
}
