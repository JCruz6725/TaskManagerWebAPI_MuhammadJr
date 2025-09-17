using Web.Api.Persistence.Models;

namespace Web.Api.Persistence.Repositories
{
    public class UserRepo
    {
        public void CreateUser(User user)
        {
            throw new NotImplementedException();

        }
        public User GetUserById(Guid Id)
        {
            throw new NotImplementedException();

        }

        private readonly AppDbContext _context;

    }
}
