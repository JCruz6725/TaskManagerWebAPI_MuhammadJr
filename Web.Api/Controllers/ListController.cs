using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Web.Api.Dto.Request;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;
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
        public async Task<ActionResult<ListDto>> CreateList([FromHeader]Guid userId, ListCreateDto createListDto)
        {
           
            
            throw new NotImplementedException();
        }

        [HttpGet("{listId}", Name = "GetListById")]
        public async Task<ActionResult<ListDto>> GetListById([FromHeader]Guid userId, Guid listId)
        {
            var getList = await _unitOfWork.List.GetListByIdAsync(listId);
            if(getList is null)
            {
                return NotFound("No List with this Id Found");
            }

            var listDtos = new ListDto
            {
                Id = getList.Id,
                Name = getList.Name,
                CreatedDate = getList.CreatedDate,
                CreatedUserId = getList.CreatedUserId,

                TaskItems = getList.TaskWithinLists.Select(twl => new TaskDto
                {
                    Id = twl.TaskItem.Id,
                    Title = twl.TaskItem.Title,
                    DueDate = twl.TaskItem.DueDate,
                    Priority = twl.TaskItem.Priority,
                    CreatedDate = twl.TaskItem.CreatedDate,
                    CreatedUserId = twl.TaskItem.CreatedUserId,
                }).ToArray()

            };
            return Ok(listDtos);
        }

        [HttpGet( Name = "GetAllList")]
        public async Task<ActionResult<List<ShortListDto>>> GetAllList([FromHeader]Guid userId)
        {
            var userLists = await _unitOfWork.List.GetAllListAsync(userId);
            if(userLists is null)
            {
               return NotFound("No Lists Found");
            }

            var getListDetail = new ListDto
            {
                Id = userLists.Id,
                Name = userLists.Name,
                CreatedDate = userLists.CreatedDate,
                CreatedUserId = userLists.CreatedUserId,

              };
            return Ok(getListDetail);
        }

        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public Task<ActionResult<ListDto>> MoveTaskToList([FromHeader]Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            throw new NotImplementedException();
        }
    }
}
