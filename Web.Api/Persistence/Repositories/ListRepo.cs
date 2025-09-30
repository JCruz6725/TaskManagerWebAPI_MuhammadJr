
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
            return await _context.Lists.Include(twl => twl.TaskWithinLists).ThenInclude(ti => ti.TaskItem).FirstOrDefaultAsync(i => i.Id == Id);
            //return await _context.Lists.Include(twl => twl.TaskWithinLists).ThenInclude(ti => ti.TaskItem).Where(i => i.Id == Id).FirstOrDefaultAsync();

        }
        public async Task<List?> GetAllListAsync(Guid Id)
        {
            return await _context.Lists.Where(cu => cu.CreatedUserId == Id).FirstOrDefaultAsync();
        }
    }
}
