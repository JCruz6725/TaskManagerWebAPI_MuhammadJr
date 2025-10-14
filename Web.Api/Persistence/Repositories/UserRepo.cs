using Microsoft.EntityFrameworkCore;
using Web.Api.Persistence.Models;

namespace Web.Api.Persistence.Repositories
{
    public class UserRepo
    { 
        private readonly TaskManagerAppDBContext _context;   //calls the the scaffolded EF Core database (All repos share this _context)
        public UserRepo (TaskManagerAppDBContext context)   //contructor for the UserRepo that sets the db context 
        {
            _context = context;    
        }
        public async Task CreateUserAsync(User user)                  //user method is created 
        {                       
           await _context.AddAsync(user);                           //users are added to the db, this method will be used to always add a new user 

        }
                                                         //method to to register user by email 
                                                         //uses LINQ to Email from Users
        public async Task<User?> GetUserByEmailAsync(string email)       
        {
           return await _context.Users.FirstOrDefaultAsync(e =>  e.Email == email);   
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(ui => ui.Id == userId);

        }
    }
}
