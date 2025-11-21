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
        private readonly StatusChange _statusChange;

        public ListController(UnitOfWork unitOfWork, IOptions<StatusChange> statusChangeOptions)
        {
            _unitOfWork = unitOfWork;
            _statusChange = statusChangeOptions.Value;
        }

        [HttpPost(Name = "CreateList")]
        public async Task<ActionResult<ListDto>> CreateList([FromHeader] Guid userId, ListCreateDto createListDto)
        {
            throw new NotImplementedException();
        }


        [HttpGet("{listId}", Name = "GetListById")]
        public async Task<ActionResult<ListDto>> GetListById([FromHeader] Guid userId, Guid listId)
        {
            if(!await _unitOfWork.User.IsUserInDbAsync(userId)) 
            {
                return StatusCode(403);
            }
            
            List? list = await _unitOfWork.List.GetListByIdAsync(listId, userId);
            if (list is null)
            {
                return NotFound(listId);
            }

            //Map List to ListDto
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
                    CurrentStatus = twl.TaskItem.TaskItemStatusHistories.OrderByDescending(s => s.CreatedDate)
                                .Select(s => new StatusDto
                                {
                                    Id = s.Status.Id,
                                    Name = s.Status.Name,
                                    Code = s.Status.Code
                                }).First(),
                    CreatedDate = twl.TaskItem.CreatedDate,
                    CreatedUserId = twl.TaskItem.CreatedUserId,
                }).ToArray(),
            };
            return Ok(listDtos);
        }

        [HttpGet(Name = "GetAllList")]
        public async Task<ActionResult<List<ShortListDto>>> GetAllList([FromHeader] Guid userId)
        {
            if(!await _unitOfWork.User.IsUserInDbAsync(userId)) 
            {
                return StatusCode(403);
            }

            List<List> userLists = await _unitOfWork.List.GetAllListAsync(userId);

            //Map List to ShortListDto
            List<ShortListDto> getListDetail = userLists.Select(sl => new ShortListDto
            {
                Id = sl.Id,
                Name = sl.Name,
                CreatedDate = sl.CreatedDate,
                CreatedUserId = sl.CreatedUserId,
            }).ToList();
            return Ok(getListDetail);
        }


        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public Task<ActionResult<ListDto>> MoveTaskToList([FromHeader] Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            throw new NotImplementedException();
        }
    }
}
