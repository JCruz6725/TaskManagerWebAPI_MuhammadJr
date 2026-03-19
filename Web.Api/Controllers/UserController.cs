using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Persistence;
using ModelLibrary;
using Web.Api.Util;

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
            using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
            {
                //checking user exists
                _logger.LogInformation("Checking login info");
                User? user = await _unitOfWork.User.GetUserByEmailAsync(resetPswDto.email);
                if (user is null)
                {
                    _logger.LogWarning($"Invalid user email: {resetPswDto.email}");
                    return BadRequest("Invalid email. Try again.");
                }

                //checking password exists
                _logger.LogInformation("Checking password");
                Password? databasePsw = (await _unitOfWork.User.GetPasswordsByIdAsync(user.Id)).FirstOrDefault();
                if (databasePsw is null)
                {
                    _logger.LogWarning($"No password exists for user: {user.Id} with the email: {resetPswDto.email}");
                    return BadRequest("No password exists for user.");
                }

                //hashing
                PasswordHasher hash = new PasswordHasher();
                string generatedSalt = hash.GenerateSalt();
                byte[] hashedOldPsw = hash.GenerateHash(resetPswDto.oldPassword, databasePsw.Salt);
                byte[] hashedNewPassword = hash.GenerateHash(resetPswDto.newPassword, generatedSalt);

                //authenticating password
                if (!hashedOldPsw.SequenceEqual(databasePsw.PasswordHash))
                {
                    _logger.LogWarning($"Invalid password for user with email: {resetPswDto.email}");
                    return Unauthorized("Invalid password");
                }

                //Email and password correct so we will create new password

                //checking that 3 previous passwords aren't being used
                _logger.LogInformation("Checking that new password is not a duplicate of last 3 password resets");
                List<Password> passwordHistory = await _unitOfWork.User.GetPasswordsByIdAsync(user.Id);
                if (passwordHistory.Count >= 3) //if user has at least 3 old passwords
                {
                    for (int i = 0; i < 3; i++) 
                    {
                        byte[] currHashPass = hash.GenerateHash(resetPswDto.newPassword, passwordHistory[i].Salt); //generate hash with the curr password's salt that we are comparing
                        if (passwordHistory[i].PasswordHash.SequenceEqual(currHashPass))
                        {
                            _logger.LogWarning($"Password \"{resetPswDto.newPassword}\" has been used before in one of the previous 3 passwords.");
                            return BadRequest($"Password has been used before. Please create a new password");
                        }
                    }
                }

                _logger.LogInformation("Adding new password");
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
    }
}
