
using Web.Api.Persistence.Models;

namespace Web.Api.Persistence.Repositories
{
    public class ListRepo
    {
        public List CreateList(List list)
        {
            throw new NotImplementedException();

        }
        public List GetListById(Guid Id)
        {
            throw new NotImplementedException();

        }
        public List GetAllList()
        {
            throw new NotImplementedException();

        }
        private readonly AppDbContext _context;
    }
}
