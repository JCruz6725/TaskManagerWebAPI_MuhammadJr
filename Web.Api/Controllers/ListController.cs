using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ValidCheck _validCheck;
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
            string validationMesasge = await _validCheck.ValidateUserListAsync(userId, listId);
            if(validationMesasge != null)
            {
                return BadRequest(validationMesasge);
            }

            List? getList = await _unitOfWork.List.GetListByIdAsync(listId);
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);

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
            return Ok(listDtos);
        }

        [HttpGet( Name = "GetAllList")]
        public async Task<ActionResult<List<ShortListDto>>> GetAllList([FromHeader]Guid userId)
        {
            string validationMessage = await _validCheck.ValidateUserAsync(userId);
            if(validationMessage != null)
            {
                return BadRequest(validationMessage);
            }

            List<List> userLists = await _unitOfWork.List.GetAllListAsync(userId);

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
        public Task<ActionResult<ListDto>> MoveTaskToList([FromHeader]Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            throw new NotImplementedException();
        }
    }
}
