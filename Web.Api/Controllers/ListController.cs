using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public ListController(UnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "CreateList")]
        public async Task<ActionResult<ListDto>> CreateList([FromHeader]Guid userId, ListCreateDto createListDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{listId}", Name = "GetListById")]
        public async Task<ActionResult<ListDto>> GetListById([FromHeader]Guid userId, int ListId)
        {
            throw new NotImplementedException();
        }

        [HttpGet( Name = "GetAllList")]
        public async Task<ActionResult<List<ShortListDto>>> GetAllList([FromHeader]Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public Task<ActionResult<ListDto>> MoveTaskToList([FromHeader]Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            throw new NotImplementedException();
        }
    }
}
