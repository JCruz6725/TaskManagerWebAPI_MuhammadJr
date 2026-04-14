using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Api.Dto.Request;
using Web.Api.Persistence;
using Web.Api.Persistence.Repositories;
using ModelLibrary;
using Web.Api.Util;
using System.Reflection.Metadata.Ecma335;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;                         //private readonly field to access the UofW class
        private readonly ILogger<UserController> _logger;
        public UserController(UnitOfWork unitOfWork, ILogger<UserController> logger)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost(Name = "RegisterUser")]                              //Http post request 
        public async Task<ActionResult<Guid>> RegisterUser(RegisterUserDto registerUserDto)     //resgister User method user creation
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating Register User method");
                    User? user = await _unitOfWork.User.GetUserByEmailAsync(registerUserDto.Email);
                    if (user is not null)
                    {
                        _logger.LogWarning($"Attempting to register with an email that is already in use: {registerUserDto.Email}");
                        return BadRequest("Email already in use.");
                    }
                    _logger.LogInformation($"Registering with email {registerUserDto.Email}");

                    //create a new instance of User thats not existing
                    //call the User props and set the registerDto to its assign props 
                    User newUser = new User
                    {
                        FirstName = registerUserDto.FirstName,
                        LastName = registerUserDto.LastName,
                        Email = registerUserDto.Email,
                        CreatedDate = DateTime.Now,
                    };
                    _logger.LogInformation("New user successfully created");
                    await _unitOfWork.User.CreateUserAsync(newUser);          //UofW takes the User class and calls the CreateUser method from the UserRepo

                    //generate hashed password
                    PasswordHasher hasher = new PasswordHasher();
                    string generatedSalt = hasher.GenerateSalt();
                    byte[] hashedPsw = hasher.GenerateHash(registerUserDto.Password, generatedSalt);
                    Password newPsw = new Password
                    {
                        PasswordHash = hashedPsw,
                        Salt = generatedSalt,
                        CreatedDate = DateTime.Now,
                        CreatedUser = newUser
                    };
                    _logger.LogInformation("Created password");
                    await _unitOfWork.User.CreatePasswordAsync(newPsw);

                    await _unitOfWork.SaveChangesAsync();                          //UofW calls the SaveChanges method
                    _logger.LogInformation($"Returning newly created user with id {newUser.Id}");
                    return Ok(newUser.Id);                                    //a new Id Guid is return once user is registered
                }
            }
            catch (Excpetion ex)
            {
                _logger.LogError($"Register user process has failed: {ex.Message}");
                return StatusCode(500);
            }
        }


        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<Guid>> Login(LoginDto userLoginDto)           //login user method creation
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
            {
                _logger.LogInformation("Initiating Login method");
                User? userLogin = await _unitOfWork.User.GetUserByEmailAsync(userLoginDto.Email);   //get user from UofW and user email from UserRepo
                if (userLogin is null)
                {
                    _logger.LogWarning($"Invalid user email: {userLoginDto.Email}");
                    return BadRequest("Invalid email, try again.");
                }

                Password? databasePsw = (await _unitOfWork.User.GetPasswordsByIdAsync(userLogin.Id)).FirstOrDefault();
                if(databasePsw is null)
                {
                    _logger.LogWarning($"No password exists for user: {userLogin.Id} with the email: {userLoginDto.Email}");
                    return BadRequest("No password exists for user.");
                }

                PasswordHasher hash = new PasswordHasher();
                byte[] hashedPsw = hash.GenerateHash(userLoginDto.Password, databasePsw.Salt);
                if (!hashedPsw.SequenceEqual(databasePsw.PasswordHash))
                {
                    _logger.LogWarning($"Invalid password for user with email: {userLoginDto.Email}");
                    return Unauthorized("Invalid password, try again");
                }

                //checking if password creation date is > 60 days ago
                _logger.LogInformation($"Checking password expiration");
                if (DateTime.Now - databasePsw.CreatedDate > TimeSpan.FromDays(60)) {
                    _logger.LogInformation("Password creation date has exceeded 60 days");
                    return Unauthorized("Password has expired, please reset the password.");
                }

                _logger.LogInformation($"User has logged in successfully: {userLoginDto.Email}");
                _logger.LogInformation($"Returning user login id {userLogin.Id}");
                return Ok(userLogin.Id); // return the registered GUID Id of that user
            }
        }

        [HttpPost("reset", Name = "ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPswDto)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Checking login info");
                    User? user = await _unitOfWork.User.GetUserByEmailAsync(resetPswDto.email);
                    if (user is null)
                    {
                        _logger.LogWarning($"Invalid user email: {resetPswDto.email}");
                        return BadRequest("Invalid email. Try again.");
                    }

                    _logger.LogInformation("Checking password");
                    Password? databasePsw = (await _unitOfWork.User.GetPasswordsByIdAsync(user.Id)).FirstOrDefault();
                    if (databasePsw is null)
                    {
                        _logger.LogWarning($"No password exists for user: {user.Id} with the email: {resetPswDto.email}");
                        return BadRequest("No password exists for user.");
                    }

                    //checking password
                    PasswordHasher hash = new PasswordHasher();
                    byte[] hashedOldPsw = hash.GenerateHash(resetPswDto.oldPassword, databasePsw.Salt);
                    if (!hashedOldPsw.SequenceEqual(databasePsw.PasswordHash))
                    {
                        _logger.LogWarning($"Invalid password for user with email: {resetPswDto.email}");
                        return Unauthorized("Invalid password");
                    }

                    //Email and password correct so we will create new password
                    _logger.LogInformation("Hashing new password");
                    string generatedSalt = hash.GenerateSalt();
                    byte[] hashedNewPassword = hash.GenerateHash(resetPswDto.newPassword, generatedSalt);
                    Password newPsw = new Password
                    {
                        PasswordHash = hashedNewPassword,
                        Salt = generatedSalt,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = user.Id,
                    };
                    _logger.LogInformation("Created new password");
                    await _unitOfWork.User.CreatePasswordAsync(newPsw);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(user.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login user process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
