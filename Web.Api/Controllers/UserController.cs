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
            User? existingUser = await _unitOfWork.User.GetUserByEmailAsync(registerUserDto.Email); //get user from UofW and user email from UserRepo
            if(existingUser != null)                                                          //check if user already exists in the database
            {
                return BadRequest("User Already Exists");
            }
                                                                         //RequestDTO
                                                                         //create a new instance of User thats not existing
                                                                        //call the User props and set the registerDto to its assign props 
            User userCreation = new User                               
            {
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Email = registerUserDto.Email,
                Password = registerUserDto.Password,
                CreatedDate = DateTime.Now,
            };

            await _unitOfWork.User.CreateUserAsync(userCreation);          //UofW takes the User class and calls the CreateUser method from the UserRepo
            await _unitOfWork.SaveChangesAsync();                          //UofW calls the SaveChanges method

            return Ok(userCreation.Id);                                    //a new Id Guid is return once user is registered
        }


        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<Guid>> Login(LoginDto userLoginDto)           //login user method creation
        {
            User? userLogin = await _unitOfWork.User.GetUserByEmailAsync(userLoginDto.Email);   //get user from UofW and user email from UserRepo
            if (userLogin == null)                                                            //if login is null send invalid
            {
                return Unauthorized("Invalid Email or Password");
            }

            if (userLogin.Password != userLoginDto.Password)                        //if password from database User does not match password from login DTO 
            {                                               
                return Unauthorized("Invalid Email or Password");                   // retunrn invalid login 
            }
            return Ok(userLogin.Id);                                     // return the registered GUID Id of that user
        }
    }
}
