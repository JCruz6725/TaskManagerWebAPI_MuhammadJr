namespace Web.Api.Dto.Request
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; } = null!;
        public int Zipcode { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string State { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Ipaddress { get; set; } = null!;


    }
}
