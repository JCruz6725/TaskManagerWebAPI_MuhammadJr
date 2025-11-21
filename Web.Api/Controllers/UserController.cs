using Microsoft.AspNetCore.Components.Forms;
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
            try
            {
                User? user = await _unitOfWork.User.GetUserByEmailAsync(registerUserDto.Email);
                if (user is not null)
                {
                    return BadRequest("Email already in use.");
                }


                //RequestDTO
                //create a new instance of User thats not existing
                //call the User props and set the registerDto to its assign props 
                User newUser = new User
                {
                    FirstName = registerUserDto.FirstName,
                    LastName = registerUserDto.LastName,
                    Email = registerUserDto.Email,
                    Password = registerUserDto.Password,
                    CreatedDate = DateTime.Now,
                };

                await _unitOfWork.User.CreateUserAsync(newUser);          //UofW takes the User class and calls the CreateUser method from the UserRepo
                await _unitOfWork.SaveChangesAsync();                          //UofW calls the SaveChanges method

                return Ok(newUser.Id);                                    //a new Id Guid is return once user is registered
            }
            catch(Exception ex)
            {
                _logger.Error($"Register user process failed: {ex.Message}");  
                return StatusCode(500);
            }
        }


        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<Guid>> Login(LoginDto userLoginDto)           //login user method creation
        {
            try
            {
                User? userLogin = await _unitOfWork.User.GetUserByEmailAsync(userLoginDto.Email);   //get user from UofW and user email from UserRepo
                if(userLogin is null || userLogin.Password != userLoginDto.Password) 
                {
                    return BadRequest("Invalid email or password.");
                }
                return Ok(userLogin.Id);  // return the registered GUID Id of that user  
            }  
            catch (Exception ex)
            {
                _logger.Error($"Login user process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
