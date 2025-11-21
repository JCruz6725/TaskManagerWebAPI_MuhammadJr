using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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


        public async Task CreateList(List list)
        {
            await _context.AddAsync(list);
        }


        public async Task<List?> GetListByIdAsync(Guid listId, Guid userId)
        {
            return await _context.Lists.Include(twl => twl.TaskWithinLists)
               .ThenInclude(ti => ti.TaskItem)
               .ThenInclude(tish => tish.TaskItemStatusHistories)
               .ThenInclude(st => st.Status)
               .SingleOrDefaultAsync(i => i.Id == listId && i.CreatedUserId == userId);
        }


        public async Task<List<List>> GetAllListAsync(Guid Id)
        {
            return await _context.Lists.Where(c => c.CreatedUserId == Id).ToListAsync();
        }

        
    }
}
