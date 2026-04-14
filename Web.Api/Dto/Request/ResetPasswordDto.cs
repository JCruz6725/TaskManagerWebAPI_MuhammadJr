namespace Web.Api.Dto.Request
{
    public class ResetPasswordDto
    {
        public string email { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
