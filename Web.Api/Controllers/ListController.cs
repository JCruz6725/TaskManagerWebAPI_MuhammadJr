using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

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
        public async Task<ActionResult<List<ListDto>>> GetAllList([FromHeader] Guid UserId)
        {
            var getLists = await _unitOfWork.List.GetAllListAsync(UserId);
            if (getLists is null)
            {
               return NotFound("No Lists Found");
            }

            var getListDetail = getLists.Select(list => new ListDto
            {
                Id = list.Id,
                Name = list.Name,
                CreatedDate = list.CreatedDate,
                CreatedUserId = list.CreatedUserId,

                //TaskItems = list.CreatedUser.TaskWithinList.Select(twl => new TaskDto
                //{
                //    Id = twl.TaskItem.Id,
                //    Title = twl.TaskItem.Title,
                //    DueDate = twl.TaskItem.DueDate,
                //    Priority = twl.TaskItem.Priority,
                //    CreatedDate = twl.TaskItem.CreatedDate,
                //    CreatedUserId = twl.TaskItem.CreatedUserId,

                //}).ToArray()

            }).ToList();

            return Ok(getListDetail);
        }

        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public void MoveTaskToList(Task task)
        {
            throw new NotImplementedException();
        }
      

    }

}
