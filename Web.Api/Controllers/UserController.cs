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
        private readonly ILogger<UserController> _logger;
        private readonly ValidCheck _validCheck;
        public UserController(UnitOfWork unitOfWork, ILogger<UserController> logger, ValidCheck validCheck)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _validCheck = validCheck;
        }

        [HttpPost(Name = "RegisterUser")]                              //Http post request 
        public async Task<ActionResult<Guid>> RegisterUser(RegisterUserDto registerUserDto)     //resgister User method user creation
        {
            _logger.LogInformation("Registering a new user");
            User? getUser = await _unitOfWork.User.GetUserByEmailAsync(registerUserDto.Email);

            string? validationMessage = _validCheck.ValidateUserRegistration(getUser);
            if (validationMessage != null)
            {
                return BadRequest(validationMessage);
            }
            _logger.LogInformation("Validation of user registration is successful");
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
            _logger.LogInformation("New user has been registered successfully");
           
            return Ok(userCreation.Id);                                    //a new Id Guid is return once user is registered
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<Guid>> Login(LoginDto userLoginDto)           //login user method creation
        {
            User? userLogin = await _unitOfWork.User.GetUserByEmailAsync(userLoginDto.Email);   //get user from UofW and user email from UserRepo

            string? validationMessage = _validCheck.ValidateUserId(userLogin);
            if (validationMessage != null)
            {
                return BadRequest(validationMessage);
            }
            _logger.LogInformation("Validation of user login is successful");
            _logger.LogInformation("User has logged in successfully");
            
            return Ok(userLogin.Id);                                     // return the registered GUID Id of that user
        }
    }
}
