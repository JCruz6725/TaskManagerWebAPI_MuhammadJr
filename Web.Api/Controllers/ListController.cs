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
        private readonly ILogger<ListController> _logger;
        public ListController (UnitOfWork unitOfWork, ILogger<ListController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        [HttpPost(Name = "CreateList")]
        public async Task<ActionResult<ListDto>> CreateList([FromHeader]Guid userId, ListCreateDto createListDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{listId}", Name = "GetListById")]
        public async Task<ActionResult<ListDto>> GetListById([FromHeader]Guid userId, Guid listId)
        {
            _logger.LogInformation("Getting list by Id");

            List? getList = await _unitOfWork.List.GetListByIdAsync(listId);
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);

            if (getList == null && getUser == null)
            {
                return NotFound($"UserId {userId} and ListId {listId} are invalid");
            }
            if (getList == null && getUser != null)
            {
                return NotFound($"ListId {listId} is invalid");
            }
            if (getList != null && getUser == null)
            {
                return NotFound($"UserId {userId} is invalid");
            }
            if (getList.CreatedUserId != getUser.Id)
            {
                return Unauthorized($"ListId {listId} does not belog to this UserId{userId} ");
            }

            ListDto listDtos = new ListDto
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
            _logger.LogInformation("Getting list successfull");
            return Ok(listDtos);
        }

        [HttpGet( Name = "GetAllList")]
        public async Task<ActionResult<List<ShortListDto>>> GetAllList([FromHeader]Guid userId)
        {
            _logger.LogInformation("Getting all list for user");

            List<List> userLists = await _unitOfWork.List.GetAllListAsync(userId);
            if(userLists == null)
            {
               return NotFound($"No Lists Found With UserId {userId} ");
            }

            List<ShortListDto> getListDetail = userLists.Select(sl => new ShortListDto
            {
                Id = sl.Id,
                Name = sl.Name,
                CreatedDate = sl.CreatedDate,
                CreatedUserId = sl.CreatedUserId,
              }).ToList();

            _logger.LogInformation("Getting ");
            return Ok(getListDetail);

        }

        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public Task<ActionResult<ListDto>> MoveTaskToList([FromHeader]Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            throw new NotImplementedException();
        }
    }
}
