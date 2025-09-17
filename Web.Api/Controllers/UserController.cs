using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Persistence.Repositories;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController
    {
        [HttpPost(Name = "RegisterUser")]
        public Guid RegisterUser(RegisterUserDto registerUserDto)
        {
            throw new NotImplementedException();

        }
        [HttpPost("login", Name = "Login")]
        public Guid Login(LoginDto userLoginDto)
        {
            throw new NotImplementedException();
        }
        private readonly UserRepo _userRepo;

    }
}
