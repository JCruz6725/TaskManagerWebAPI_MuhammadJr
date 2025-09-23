
using Web.Api.Persistence.Models;

namespace Web.Api.Persistence.Repositories
{
    public class ListRepo
    {
        private readonly TaskManagerAppDBContext _context;
        public ListRepo(TaskManagerAppDBContext context)
        {
            _context = context;
        }

        public List CreateList(List list)
        {
            throw new NotImplementedException();

        }
        public async Task<List> GetListByIdAsync(Guid Id)
        {
            throw new NotImplementedException();

        }
        public List GetAllList()
        {
            throw new NotImplementedException();

        }
    }
}
