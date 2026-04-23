using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using ModelLibrary;
using System.Reflection.Metadata.Ecma335;
using Web.Api.Dto.Request;
using Web.Api.Persistence;
using Web.Api.Persistence.Repositories;
using Web.Api.scaffolding_temp_folder;
using Web.Api.Util;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;                         //private readonly field to access the UofW class
        private readonly PurposeTypeOptions purposeTypeOptions;
        private readonly LicenseTypeOptions licenseTypeOptions;
        private readonly ILogger<UserController> _logger;

        public UserController(UnitOfWork unitOfWork, ILogger<UserController> logger,
            IOptions<LicenseTypeOptions> licenseOptions,
            IOptions<PurposeTypeOptions> purposeOptions)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            purposeTypeOptions = purposeOptions.Value;
            licenseTypeOptions = licenseOptions.Value;
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
                        return BadRequest("Email already in use, please use a different email.");
                    }

                    //checking if password policy passes
                    VerifyPasswordPolicy passpPolicy = new VerifyPasswordPolicy();
                    if (!passpPolicy.Verify(registerUserDto.Password))
                    {
                        _logger.LogWarning($"Password '{registerUserDto.Password}' does not comply with the password policy");
                        return BadRequest("Password policy failed. Please create a password that complies");
                    }
                    _logger.LogInformation("Password Policy Passed");

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
            catch (Exception ex)
            {
                _logger.LogError($"Register user process has failed: {ex.Message}");
                return StatusCode(500);
            }
        }


        [HttpPost( "ExtraInfo")]
        public async Task<ActionResult<Guid>> ExtraInfo([FromHeader]Guid userId , [FromBody] NewExtraInfoDto newExtraInfoDto)     //resgister User method user creation
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
            {
                _logger.LogInformation("Intiating ExtraInfo Method");

                if (newExtraInfoDto == null)
                {
                    _logger.LogWarning("ExtraInfo payload is null");
                    return BadRequest("Invalid Payload ");
                }
                _logger.LogInformation($"Fetching users with Id{userId}");
                User? user = await _unitOfWork.User.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User not found with id {userId}");
                     return NotFound("User Not Found");
                 }
                _logger.LogInformation($"Resolving license type for :{newExtraInfoDto.LicenseTitle}");
                Guid licenseId = newExtraInfoDto.LicenseTitle.ToLower() switch
                {
                    "paid" => licenseTypeOptions.PaidId,
                    "free" => licenseTypeOptions.FreeId
                };

                _logger.LogInformation($"Resolving purpose type for:{newExtraInfoDto.PurposeTitle}");  
                Guid purposeId = newExtraInfoDto.PurposeTitle.ToLower() switch
                {
                    "work" => purposeTypeOptions.WorkId,
                    "education" => purposeTypeOptions.EducationId,
                    "personal" => purposeTypeOptions.PersonalId
                };

                _logger.LogInformation($"Creating address for user {userId}");
                Address address = new Address
                {
                    Address1 = newExtraInfoDto.Address1,
                    City = newExtraInfoDto.City,
                    State = newExtraInfoDto.State,
                    Zipcode = newExtraInfoDto.ZipCode,
                    CreatedUserId = user.Id
                };
                _logger.LogInformation($"Creating profile for user {userId}");
                Profile newProfile = new Profile
                {
                    DateOfBirth = newExtraInfoDto.DateOfBirth,
                    PhoneNumber = newExtraInfoDto.PhoneNumber,
                    Gender = newExtraInfoDto.Gender,
                    Education = newExtraInfoDto.Education,
                    Employer =  newExtraInfoDto.Employer,
                    JobTitle = newExtraInfoDto.JobTitle,
                    CreatedUserId = user.Id,
                    LicenseId = licenseId,
                    PurposeId = purposeId
                };
               
                await _unitOfWork.User.CreateProfileAsync(newProfile);
                _logger.LogInformation($"Profile succesfully create for user {userId}");
                await _unitOfWork.User.CreateAddressAsync(address);
                _logger.LogInformation($"Address succesfully created for user{userId}");
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"ExtraInfo saved succesfully, returning the userId {userId}");

                return Ok(userId);                         
            }
        }


 [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<RequireExtraInfoFlagDto>> Login(LoginDto userLoginDto)           //login user method creation
        {
            try
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
                    if (databasePsw is null)
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
                        _logger.LogWarning("Password creation date has exceeded 60 days");
                        return Unauthorized("Password has expired, please reset the password.");
                    }
                    _logger.LogInformation("Checking if existing user has a profile");
                    bool hasExtraInfo = await _unitOfWork.User.HasExtraInfoAsync(userLogin.Id);
                    bool requiresExtraInfo = !hasExtraInfo;

                    _logger.LogInformation($"User has logged in successfully: {userLoginDto.Email}");
                    _logger.LogInformation($"Returning user login id {userLogin.Id}");
                    return Ok(new RequireExtraInfoFlagDto
                    {
                        UserId = userLogin.Id,
                        RequiresExtraInfo = requiresExtraInfo
                    }); // return the registered GUID Id of that user
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"User log in process has failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("reset", Name = "ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPswDto)
        {
            try
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

                    PasswordHasher hash = new PasswordHasher();
                    string generatedSalt = hash.GenerateSalt();
                    byte[] hashedOldPsw = hash.GenerateHash(resetPswDto.oldPassword, databasePsw.Salt); //uses salt stored in db
                    byte[] hashedNewPassword = hash.GenerateHash(resetPswDto.newPassword, generatedSalt); //uses newly created salt

                    //authenticating password
                    if (!hashedOldPsw.SequenceEqual(databasePsw.PasswordHash))
                    {
                        _logger.LogWarning($"Invalid password for user with email: {resetPswDto.email}");
                        return Unauthorized("Invalid password");
                    }

                    //Email and password correct so we will create new password
                    //check password policy passes for new password
                    _logger.LogInformation("Verifying password policy passes");
                    VerifyPasswordPolicy passwordPolicy = new VerifyPasswordPolicy();
                    if (!passwordPolicy.Verify(resetPswDto.newPassword))
                    {
                        _logger.LogWarning($"Password '{resetPswDto.newPassword}' does not comply with the password policy");
                        return BadRequest("Password policy failed. Please create a password that complies");
                    }

                    //checking that 3 previous passwords aren't being used
                    _logger.LogInformation("Checking that new password is not a duplicate of last 3 password resets");
                    List<Password> passwordHistory = await _unitOfWork.User.GetPasswordsByIdAsync(user.Id);
                    if (passwordHistory.Count >= 3) //if user has at least 3 old passwords
                    {
                        byte[] currHashedPass;
                        for (int i = 0; i < 3; i++)
                        {
                            currHashedPass = hash.GenerateHash(resetPswDto.newPassword, passwordHistory[i].Salt); //generate hash using curr password's salt that we are comparing
                            if (passwordHistory[i].PasswordHash.SequenceEqual(currHashedPass))
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
            catch (Exception ex)
            {
                _logger.LogError($"Login user process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
