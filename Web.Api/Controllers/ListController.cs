using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
        public ListController(UnitOfWork unitOfWork, IOptions<StatusChange> statusChangeOptions, ILogger<ListController> logger)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost(Name = "CreateList")]
        public async Task<ActionResult<ListDto>> CreateList([FromHeader] Guid userId, ListCreateDto createListDto)
        {
            throw new NotImplementedException();
        }


        [HttpGet("{listId}", Name = "GetListById")]
        public async Task<ActionResult<ListDto>> GetListById([FromHeader] Guid userId, Guid listId)
        {
            _logger.LogInformation("Innitiating GetListById");
            if(!await _unitOfWork.User.IsUserInDbAsync(userId)) 
            {
                _logger.LogWarning($"UserId {userId} not found in database");
                return StatusCode(403);
            }
            
            List? list = await _unitOfWork.List.GetListByIdAsync(listId, userId);
            if (list is null)
            {
                _logger.LogWarning($"ListId {listId} not found for UserId {userId}");
                return NotFound(listId);
            }

            ListDto listDtos = new ListDto
            {
                Id = list.Id,
                Name = list.Name,
                CreatedDate = list.CreatedDate,
                CreatedUserId = list.CreatedUserId,

                TaskItems = list.TaskWithinLists.Select(twl => new TaskDto
                {
                    Id = twl.TaskItem.Id,
                    Title = twl.TaskItem.Title,
                    DueDate = twl.TaskItem.DueDate,
                    Priority = twl.TaskItem.Priority,
                    CreatedDate = twl.TaskItem.CreatedDate,
                    CreatedUserId = twl.TaskItem.CreatedUserId,
                }).ToArray()

            };
           _logger.LogInformation($"GetListById method successful for ListId {listId} and UserId {userId}");
            _logger.LogInformation("Returning get list by Id result");
            return Ok(listDtos);
        }

        [HttpGet(Name = "GetAllList")]
        public async Task<ActionResult<List<ShortListDto>>> GetAllList([FromHeader] Guid userId)
        {
            _logger.LogInformation("Innitiating GetAllList");
            if (!await _unitOfWork.User.IsUserInDbAsync(userId)) 
            {
                _logger.LogWarning($"UserId {userId} not found in database");
                return StatusCode(403);
            }

            List<List> userLists = await _unitOfWork.List.GetAllListAsync(userId);

            List<ShortListDto> getListDetail = userLists.Select(sl => new ShortListDto
            {
                Id = sl.Id,
                Name = sl.Name,
                CreatedDate = sl.CreatedDate,
                CreatedUserId = sl.CreatedUserId,
            }).ToList();

            _logger.LogInformation($"GetAllList method successful for UserId {userId}");
            _logger.LogInformation("Returning get all lists result");
            return Ok(getListDetail);
        }


        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public Task<ActionResult<ListDto>> MoveTaskToList([FromHeader] Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            throw new NotImplementedException();
        }
    }
}
