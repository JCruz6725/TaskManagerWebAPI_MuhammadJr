using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;
using Web.Api.Persistence.Repositories;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;                         //private readonly field to access the UofW class

        public UserController(UnitOfWork unitOfWork)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "RegisterUser")]                              //Http post request 
        public async Task<ActionResult<Guid>> RegisterUser(RegisterUserDto registerUserDto)     //resgister User method user creation
        {
            var existingUser = await _unitOfWork.User.GetUserByEmailAsync(registerUserDto.Email);
            if(existingUser is not null)
            {
                return BadRequest("User Already Exists");
            }
                                                                     //create a new instance of User thats not existing
                                                                    //call the User props and set the registerDto to its assign props 
            var userCreation = new User                               
            {
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Email = registerUserDto.Email,
                Password = registerUserDto.Password,
                CreatedDate = DateTime.Now,
            };

            await _unitOfWork.User.CreateUserAsync(userCreation);          //UofW takes the User class and calls the CreateUser method from the UserRepo
            await _unitOfWork.SaveChangesAsync();                          //OofW calls the SaveChanges method

            return Ok(userCreation.Id);                             //a new Id Guid is return once user is registered
        }

        [HttpPost("login", Name = "Login")]
        public Guid Login(LoginDto userLoginDto)
        {
            throw new NotImplementedException();
        }
    }
}
