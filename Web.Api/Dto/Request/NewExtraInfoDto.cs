namespace Web.Api.Dto.Request
{
    public class NewExtraInfoDto
    {

        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Education { get; set; }
        public string Employer { get; set; }
        public string JobTitle { get; set; }

        public string LicenseTitle { get; set; }
        public string PurposeTitle { get; set; }
    }
}
