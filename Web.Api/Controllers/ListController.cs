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
            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            List? createList = new List
            {
                Id = Guid.NewGuid(),
                Name = createListDto.Name,
                CreatedDate = DateTime.Now,
                CreatedUserId = userId,
            };

            await _unitOfWork.List.CreateList(createList);   // add the list // sending information to the database 
            await _unitOfWork.SaveChangesAsync();

            ListDto listDtos = new ListDto     // should we use shortlistDto?
            {
                Id = createList.Id,
                Name = createList.Name,
                CreatedDate = createList.CreatedDate,
                CreatedUserId = createList.CreatedUserId,

                TaskItems = []
            
            };

               return Ok(listDtos);   
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
       
        [HttpPut("{listId}/edit-list", Name = "Edit List")]
        public async Task<ActionResult<ListDto>> EditList([FromHeader] Guid userId, Guid listId, EditListDto editListDto)
        {
            if (!await _unitOfWork.User.IsUserInDbAsync(userId)) { return StatusCode(403); }

            List? userList = await _unitOfWork.List.GetListByIdAsync(listId, userId);
            if (userList != null)
            {
                userList.Name = editListDto.NewListTitle;
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                return BadRequest("List does not exist");
            }

            EditListResDto editListResDto = new EditListResDto
            {
                Id = listId,
                Name = userList.Name,
                CreatedUserId = userId,
            };

            return Ok(editListResDto);
        }
    }
}
