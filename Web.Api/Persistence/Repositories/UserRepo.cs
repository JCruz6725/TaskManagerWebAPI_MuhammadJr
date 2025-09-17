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
        public void CreateUser(User user)                  //user method is created 
        {                       
            _context.Add(user);                           //users are added to the db, this method will be used to always add a new user 

        }
                                                         //method to to register user by email 
                                                         //uses LINQ to Email from Users
        public User? GetUserByEmail(string email)       
        {
           return _context.Users.FirstOrDefault(x =>  x.Email == email);   

        }

    }
}
