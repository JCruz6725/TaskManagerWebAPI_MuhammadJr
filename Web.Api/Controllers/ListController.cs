using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using Web.Api.Persistence.Repositories;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ListController (UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "CreateList")]
        public ListDto CreateList(ListCreateDto createListDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{listId}", Name = "GetListById")]
        public ListDto GetListById(int ListId)
        {
            


            throw new NotImplementedException();
        }

        [HttpGet( Name = "GetAllList")]
        public async Task<ActionResult<List<ListDto>>> GetAllListAsync([FromHeader] Guid UserId)
        {
            var lists = await _unitOfWork.List.GetListByIdAsync(UserId);
            if(lists is null)
            {
               return NotFound("No Lists Found");
            }

            var listDtos = new ListDto
            {
                Name = lists.Name,
                CreatedDate = lists.CreatedDate,
                CreatedUserId = lists.CreatedUserId,
              };

            return Ok(listDtos);
        }

        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public void MoveTaskToList(Task task)
        {
            throw new NotImplementedException();
        }
      

    }

}
