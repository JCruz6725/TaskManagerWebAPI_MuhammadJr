
using Microsoft.EntityFrameworkCore;
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
        public async Task<List?> GetListByIdAsync(Guid Id)
        {


            throw new NotImplementedException();

        }
        public async Task<List<List>> GetAllListAsync(Guid Id)
        {
            return await _context.Lists.Where(create => create.CreatedUserId == Id).ToListAsync();
            //return await _context.Lists.Include(twl => twl.CreatedUser.)
            //    .ThenInclude(task => task.TaskItem).Where(create => create.CreatedUserId == Id).ToListAsync();
        }
    }
}
