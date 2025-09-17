
using Web.Api.Persistence.Models;

namespace Web.Api.Persistence.Repositories
{
    public class ListRepo
    {
        public ListRepo(TaskManagerAppDBContext context)
        {
            _context = context;
        }

        private readonly TaskManagerAppDBContext _context;

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
    }
}
