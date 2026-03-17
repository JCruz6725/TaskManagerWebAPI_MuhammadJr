using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
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
                //RequestDTO
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
                    _logger.LogWarning($"Invalid user login: {userLoginDto.Email} or Password: {userLoginDto.Password}");
                    return BadRequest("Invalid email or password.");
                }


                _logger.LogInformation($"User has logged in successfully: {userLoginDto.Email}");
                _logger.LogInformation($"Returning user login id {userLogin.Id}");
                return Ok(userLogin.Id);                                     // return the registered GUID Id of that user
            }
        }

        [HttpPost("device", Name = "Device")]
        public async Task<ActionResult<Guid>> Device(Guid userId)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))

            {
                _logger.LogInformation("Initiating Login Succesful");
                if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                {
                    _logger.LogWarning($"UserId {userId} not authorized");
                    return StatusCode(403);

                }
                 var rawIp = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? HttpContext.Connection.RemoteIpAddress?.ToString();

                var ip = "unknown";
                    if(!string.IsNullOrEmpty(rawIp))
                {
                    if (rawIp.Contains("::1"))
                        ip = "127.0.0.1";
                    else if (rawIp.Contains("127.0.0.1"))
                        ip = "127.0.0.1";
                }

                var secchua = Request.Headers["sec-ch-ua"].ToString();

                var browser = "unknown";

                if (!string.IsNullOrEmpty(secchua))
                {
                    if (secchua.Contains("Microsoft Edge"))
                        browser = "Microsoft Edge ";
                    else if (secchua.Contains("Google Chrome"))
                        browser = "Google Chrome";
                   
                }
                else
                {
                    var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                    
                    if (userAgent.Contains("Firefox"))
                        browser = "Firefox";
                    
                }

                var device = new DeviceDatum
                    {
                        Id = Guid.NewGuid(),
                        IpAddress = ip,
                        BrowserType = browser,
                        AccessTime = DateTime.UtcNow,
                        CreatedUserId = userId

                    }; 
                await _unitOfWork.User.CreateAsync(device);

                return Ok(new
                {
                    IpAddress = ip, 
                    BrowserType =browser, 
                    AccessTime = DateTime.UtcNow, 
                    
                }); 

            }
        }
    }
}

        
    

