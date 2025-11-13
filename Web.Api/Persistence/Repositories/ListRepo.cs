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


        public async Task<List?> GetListByIdAsync(Guid listId, Guid userId)
        {
            return await _context.Lists.Include(twl => twl.TaskWithinLists)
                .ThenInclude(ti => ti.TaskItem)
                .SingleOrDefaultAsync(i => i.Id == listId && i.CreatedUserId == userId);
        }

        public async Task<List<List>> GetAllListAsync(Guid Id)
        {
            return await _context.Lists.Where(c => c.CreatedUserId == Id).ToListAsync();
        }

        
    }
}
