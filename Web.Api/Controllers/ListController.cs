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
        public ListController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        public async Task<ActionResult<ListDto>> MoveTaskToList([FromHeader] Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            if (!await _unitOfWork.User.IsUserInDbAsync(userId))
            {
                return StatusCode(403);
            }

            TaskItem? userTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskListMoveDto.TaskId, userId);
            List? userList = await _unitOfWork.List.GetListByIdAsync(listId, userId);
            if (userTask != null && userList != null)
            {
                if (userTask.TaskWithinLists.Count == 0) //task not assigned to a list
                {
                    userList.TaskWithinLists.Add(
                        new TaskWithinList()
                        {
                            TaskListId = listId,
                            TaskItemId = userTask.Id,
                            CreatedDate = userTask.CreatedDate,
                            CreatedUserId = userId
                        }
                    );
                }
                //need to check if task is already in the list user wants to put it in 
                else if (userTask.TaskWithinLists.ElementAt(0).TaskList == userList)
                {
                    return BadRequest("Task already exist in the list");
                }
                else //task is in a different preexisting list
                {
                    userTask.TaskWithinLists.Clear(); //Remove list from task
                    //Remove task from list
                    List<List> list = await _unitOfWork.List.GetAllListAsync(userId);
                    (list.ElementAt(0)).TaskWithinLists.Clear();

                    userTask.TaskWithinLists = [
                        new TaskWithinList(){
                            TaskListId = listId,
                            CreatedDate = userTask.CreatedDate,
                            CreatedUserId = userTask.CreatedUserId,
                        }
                    ];
                }
                await _unitOfWork.SaveChangesAsync();
            }
            else if (userList == null)
            {
                return BadRequest($"Requested list does not exist for user {userId}");
            }

            return Ok(listId);
        }
    }
}
