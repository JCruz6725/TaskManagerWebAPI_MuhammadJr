using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly ILogger<ListController> _logger;
        public ListController(UnitOfWork unitOfWork, IOptions<StatusChange> statusChangeOptions, ILogger<ListController> logger)                    //constructor for the UofW that acceses the private field
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost(Name = "CreateList")]
        public async Task<ActionResult<ListDto>> CreateList([FromHeader] Guid userId, ListCreateDto createListDto)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation($"Initiating Create List method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"User {userId} not authorized");
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
                    _logger.LogInformation($"List creation is successful for user {userId}");

                    ListDto listDtos = new ListDto     // should we use shortlistDto?
                    {
                        Id = createList.Id,
                        Name = createList.Name,
                        CreatedDate = createList.CreatedDate,
                        CreatedUserId = createList.CreatedUserId,

                        TaskItems = []

                    };
                    _logger.LogInformation($"Returning the newly created list for user {userId}");
                    return Ok(listDtos);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create list process failed: {ex.Message}");
                return StatusCode(500);
            }
        }


        [HttpGet("{listId}", Name = "GetListById")]
        public async Task<ActionResult<ListDto>> GetListById([FromHeader] Guid userId, Guid listId)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Innitiating GetListById");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get list by id process failed: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet(Name = "GetAllList")]
        public async Task<ActionResult<List<ShortListDto>>> GetAllList([FromHeader] Guid userId)
        {
            try
            { 
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Innitiating GetAllList");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                    {
                        _logger.LogWarning($"UserId {userId} not authorized");
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get all lists process failed: {ex.Message}");
                return StatusCode(500);
            }
        }


        [HttpPost("{listId}/move-task", Name = "MoveTaskToList")]
        public Task<ActionResult<ListDto>> MoveTaskToList([FromHeader] Guid userId, Guid listId, TaskListMoveDto taskListMoveDto)
        {
            throw new NotImplementedException();
        }
       
        [HttpPut("{listId}/edit-list", Name = "Edit List")]
        public async Task<ActionResult<ListDto>> EditList([FromHeader] Guid userId, Guid listId, EditListDto editListDto)
        {
            try
            {
                using (_logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    _logger.LogInformation("Initiating Edit List Method");
                    if (!await _unitOfWork.User.IsUserInDbAsync(userId)) {
                        _logger.LogInformation($"UserId {userId} not authorized");
                        return StatusCode(403); 
                    }

                    List? userList = await _unitOfWork.List.GetListByIdAsync(listId, userId);
                    if (userList != null)
                    {
                        userList.Name = editListDto.Title;
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogWarning($"List Id {listId} not found for user {userId}");
                        return NotFound(listId);
                    }
                    _logger.LogInformation($"Edit list is successful for user {userId}");

                    EditListResDto editListResDto = new EditListResDto
                    {
                        Id = listId,
                        Name = userList.Name,
                        CreatedUserId = userId,
                    };
                    _logger.LogInformation($"Returning the newly edited list for user {userId}");
                    return Ok(editListResDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Edit list process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
 

        [HttpDelete("{listId}", Name = "DeleteList")]
        public async Task<ActionResult<ListDto>> DeleteList([FromHeader] Guid userId, Guid listId)
        {
            try
            {
                if (!await _unitOfWork.User.IsUserInDbAsync(userId))
                {
                    return StatusCode(403);
                }

                List? list = await _unitOfWork.List.GetListByIdAsync(listId, userId);
                if (list is null)
                {
                    return NotFound(listId);
                }
                //checks if there is any items within the list being deleted. 
                if (list.TaskWithinLists.Any())
                {
                    return BadRequest();
                }

                _unitOfWork.List.DeleteList(list);
                await _unitOfWork.SaveChangesAsync();

                ListDto deletelist = new ListDto
                {
                    Id = list.Id,
                    Name = list.Name,
                    CreatedDate = list.CreatedDate,
                    CreatedUserId = list.CreatedUserId,
                    TaskItems = []
                };
                return Ok(deletelist);  // fix the returnvalue 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete list process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
