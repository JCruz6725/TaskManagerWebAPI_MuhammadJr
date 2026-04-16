using Microsoft.EntityFrameworkCore;
using ModelLibrary;
using Web.Api.scaffolding_temp_folder;

namespace Web.Api.Persistence.Repositories
{
    public class UserRepo
    {
        private readonly TaskManagerAppDBContext _context;   //calls the the scaffolded EF Core database (All repos share this _context)


        public UserRepo(TaskManagerAppDBContext context)   //contructor for the UserRepo that sets the db context 
        {
            _context = context;
        }


        public async Task CreateUserAsync(User user)                  //user method is created 
        {                       
           await _context.AddAsync(user);                           //users are added to the db, this method will be used to always add a new user 
        }

        public async Task CreatePasswordAsync(Password password)
        {
            await _context.AddAsync(password);
        }
        //method to to register user by email 
        //uses LINQ to Email from Users
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }


        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(ui => ui.Id == userId);
        }

        public async Task<List<Password>> GetPasswordsByIdAsync(Guid userId)
        {

            return await _context.Passwords.Where(ui => ui.CreatedUserId == userId).OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        public async Task<DeviceDatum> CreateAsync(DeviceDatum device)
        {
            await _context.DeviceData.AddAsync(device);
            return device;
        }
        
        //method to check if user exists in db by Id
        public async Task<bool> IsUserInDbAsync(Guid userId) => await _context.Users.AnyAsync(u => u.Id == userId);
    }
}

 

